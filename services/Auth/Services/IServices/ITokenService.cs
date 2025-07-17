using System.Security.Claims;
using Auth.Contracts.Dtos.Auth;

namespace Auth.Services.IServices;

public interface ITokenService
{
    Task<TokensResponse> GenerateTokensAsync(Guid userId, string email, IEnumerable<string> roles);
    Task<TokensResponse> RefreshTokenAsync(string refreshToken);
    UserClaims GetUserClaims(ClaimsPrincipal principal);
    Task<UserInfo> GetUserInfoAsync(ClaimsPrincipal principal);
}