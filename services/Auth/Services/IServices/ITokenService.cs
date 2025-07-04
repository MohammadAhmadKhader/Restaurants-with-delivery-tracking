using System.Security.Claims;
using Auth.Contracts.Dtos.Auth;

namespace Auth.Services.IServices;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    UserClaims GetUserClaims(ClaimsPrincipal principal);
}