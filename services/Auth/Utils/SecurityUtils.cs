using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Config;
using Auth.Models;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Utils;

static class SecurityUtils
{
    public static TokenValidationParameters CreateValidationTokenParams(JwtSettings jwtSettings, SymmetricSecurityKey securityKey)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
        };
    }

    public static TokenValidationParameters CreateValidationTokenParams(JwtSettings jwtSettings)
    {
        var key = CreateSecretKey(jwtSettings.SecretKey);
        return CreateValidationTokenParams(jwtSettings, key);
    }
    private static SymmetricSecurityKey CreateSecretKey(string secretKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    }

    public static Guid ExtractUserId(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Guid.Empty;
        }

        var isSuccess = Guid.TryParse(userIdClaim.Value, out var userId);
        if (!isSuccess)
        {
            return Guid.Empty;
        }

        return userId;
    }

    public static bool IsSuperAdminOnly(Permission permission)
    {
        return !permission.IsDefaultUser &&
            !permission.IsDefaultAdmin &&
        permission.IsDefaultSuperAdmin;
    }

    public static bool IsOwnerOnly(RestaurantPermission permission)
    {
        return !permission.IsDefaultUser &&
            !permission.IsDefaultAdmin &&
        permission.IsDefaultOwner;
    }

    public static bool IsSuperAdminRole(Role role) => role.NormalizedName == RolePolicies.SuperAdmin;
    public static bool IsAdminRole(Role role) => role.NormalizedName == RolePolicies.Admin;
    public static bool IsUserRole(Role role) => role.NormalizedName == RolePolicies.User;

    public static bool IsRestOwnerRole(RestaurantRole role) => role.NormalizedName == RolePolicies.RestaurantOwner;
    public static bool IsRestCustomerRole(RestaurantRole role) => role.NormalizedName == RolePolicies.RestaurantAdmin;
    public static bool IsRestAdminRole(RestaurantRole role) => role.NormalizedName == RolePolicies.RestaurantCustomer;

    public static string GenerateSecureToken(int length = 32)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}