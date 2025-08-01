
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Config;
using Auth.Contracts.Dtos.Auth;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Auth.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Exceptions;

namespace Auth.Services;

public class TokenService : ITokenService
{
    private const string TenantKey = "tenantId";
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly SecurityKey _securityKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IUsersService _usersService;
    private readonly ITenantProvider _tenantProvider;

    public TokenService(
        IOptions<JwtSettings> jwtSettings,
        ILogger<TokenService> logger,
        IRefreshTokenRepository refreshTokenRepository,
        IUsersService usersService,
        ITenantProvider tenantProvider)
    {
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;

        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);
        _tokenHandler = new JwtSecurityTokenHandler();

        ValidateConfiguration();

        _usersService = usersService;
        _tenantProvider = tenantProvider;
    }

    public async Task<TokensResponse> GenerateTokensAsync(Guid userId, string email, IEnumerable<string> roles)
    {
        try
        {
            var accessToken = GenerateAccessToken(userId, email, roles);
            var refreshToken = GenerateRefreshToken();

            await _refreshTokenRepository.StoreRefreshTokenAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = userId.ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            _logger.LogInformation("Token generated successfully for user {UserId}", userId);

            return new TokensResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user {UserId}", userId);
            throw;
        }
    }

    public async Task<TokensResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (storedToken == null || !storedToken.IsActive || storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid or expired refresh token");
            }

            await _refreshTokenRepository.RevokeRefreshTokenAsync(refreshToken);
            var userClaims = await GetUserClaimsAsync(Guid.Parse(storedToken.UserId));

            return await GenerateTokensAsync(
                Guid.Parse(storedToken.UserId),
                userClaims.Email,
                userClaims.Roles
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }

    private string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (_tenantProvider.RestaurantId != null)
        {
            claims.Add(new(TenantKey, _tenantProvider.RestaurantId.ToString()!));
        }

        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = _signingCredentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var jwtString = _tokenHandler.WriteToken(token);

        return jwtString;
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_jwtSettings.SecretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is required");
        }

        if (_jwtSettings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
        }

        if (string.IsNullOrEmpty(_jwtSettings.Issuer))
        {
            throw new InvalidOperationException("JWT Issuer is required");
        }

        if (string.IsNullOrEmpty(_jwtSettings.Audience))
        {
            throw new InvalidOperationException("JWT Audience is required");
        }
    }

    private async Task<UserClaims> GetUserClaimsAsync(Guid userId)
    {
        var user = await _usersService.FindByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with id: {userId} was not found");
        }

        return new UserClaims
        {
            UserId = user.Id,
            Email = user.Email!,
            Roles = user.Roles.Select(r => r.Name).ToHashSet()!
        };
    }

    public UserClaims GetUserClaims(ClaimsPrincipal principal)
    {
        var hasParsed = Guid.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId);
        if (!hasParsed)
        {
            // if claims have no userId and the user is authenticated that has to be internal server error
            // or some auth error that must have the result hiddeen fro security purposes
            throw new InternalServerException($"Failed to parse {nameof(ClaimTypes.NameIdentifier)}");
        }

        var claims = new UserClaims
        {
            UserId = parsedUserId,
            Email = principal.FindFirst(ClaimTypes.Email)?.Value!,
            Roles = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToHashSet()
        };

        return claims;
    }

    public async Task<UserInfo> GetUserInfoAsync(ClaimsPrincipal principal)
    {
        var hasParsed = Guid.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId);
        if (!hasParsed)
        {
            throw new InternalServerException($"Failed to parse {nameof(ClaimTypes.NameIdentifier)}");
        }

        var restaurantId = principal.FindFirst(TenantKey)?.Value;
        var user = await GetUserAsync(restaurantId, parsedUserId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with id: {parsedUserId} was not found");
        }

        var (roles, permissions) = GetUserRolesAndPermissions(restaurantId, user);

        return new UserInfo
        {
            UserId = user.Id,
            Roles = roles,
            Permissions = permissions,
            RestaurantId = user.RestaurantId,
        };
    }

    private async Task<User?> GetUserAsync(string? restaurantId, Guid userId)
    {
        if (restaurantId == null)
        {
            return await _usersService.FindByIdWithRolesAndPermissionsAsync(userId);
        }

        return await _usersService.FindByIdWithRestaurantRolesAndPermissionsAsync(userId);
    }

    private static (HashSet<string> roles, HashSet<string> permissions) GetUserRolesAndPermissions(string? restaurantId, User user)
    {
        if (restaurantId == null)
        {
            return (
                user.Roles.Select(r => r.Id.ToString()).ToHashSet(),
                user.Roles.SelectMany(r => r.Permissions).Select(r => r.Name).ToHashSet()
            );
        }

        return (
            user.RestaurantRoles.Select(r => r.Id.ToString()).ToHashSet(),
            user.RestaurantRoles.SelectMany(r => r.Permissions).Select(r => r.NormalizedName).ToHashSet()
        );
    }
}