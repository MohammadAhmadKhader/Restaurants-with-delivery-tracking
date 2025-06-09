
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Config;
using Auth.Dtos;
using Auth.Dtos.Auth;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Services;
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<TokenService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly SecurityKey _securityKey;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly IUsersService _usersService;

    public TokenService(IConfiguration configuration, ILogger<TokenService> logger,
    IRefreshTokenRepository refreshTokenRepository, IUsersService usersService)
    {
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;

        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        _tokenHandler = new JwtSecurityTokenHandler();

        ValidateConfiguration();

        _usersService = usersService;
    }

    public async Task<TokenResponse> GenerateTokenAsync(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        try
        {
            var accessToken = GenerateAccessToken(userId, email, roles, permissions);
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

            return new TokenResponse
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

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
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
            
            return await GenerateTokenAsync(
                Guid.Parse(storedToken.UserId),
                userClaims.Email,
                userClaims.Roles,
                userClaims.Permissions
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }

    private string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        if (permissions != null)
        {
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject            = new ClaimsIdentity(claims),
            Expires            = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = _signingCredentials,
            Issuer             = _jwtSettings.Issuer,
            Audience           = _jwtSettings.Audience
        };
        var token     = _tokenHandler.CreateToken(tokenDescriptor);
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
        if (user == null) {
            throw new InvalidOperationException($"User with id: {userId} was not found");
        }

        return new UserClaims
        {
            Email = user.Email!,
            Roles = user.Roles.Select(r => r.Name).ToHashSet()!,
            Permissions = user.Roles.SelectMany(r => r.Permissions.Select(p => p.Name)).Distinct().ToHashSet()
        };
    }
}