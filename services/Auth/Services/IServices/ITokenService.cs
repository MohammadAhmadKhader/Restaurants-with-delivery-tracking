using Auth.Dtos;
using Auth.Dtos.Auth;

namespace Auth.Services.IServices;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
}