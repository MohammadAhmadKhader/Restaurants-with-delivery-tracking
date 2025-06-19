
using Auth.Contracts.Dtos.Auth;

namespace Auth.Contracts.Clients;

public interface IAuthServiceClient
{
    Task<UserWithTokensDto> LoginAsync(LoginDto dto);
    Task<UserWithTokensDto> RegisterAsync(RegisterDto dto);
    Task<object> ResetPasswordAsync(ResetPasswordDto dto);
    Task<object> TestAsync(object data);
}