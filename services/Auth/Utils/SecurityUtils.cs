using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Dtos;
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
}