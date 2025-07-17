using Auth.Contracts.Dtos.Auth;
using Refit;

namespace Auth.Contracts.Clients;

public interface IAuthServiceClient
{
    [Post("/api/auth/login")]
    Task<UserWithTokensDto> LoginAsync([Body] LoginDto dto);

    [Post("/api/auth/register")]
    Task<UserWithTokensDto> RegisterAsync([Body] RegisterDto dto);

    [Post("/api/auth/reset-password")]
    Task<ApiResponse<object>> ResetPasswordAsync([Body] ResetPasswordDto dto);

    [Post("/api/auth/refresh")]
    Task<RefreshResponseDto> Refresh([Body] RefreshRequest dto);

    [Get("/api/auth/user-info")]
    Task<UserInfo> GetUserInfoAsync();

    [Post("/api/auth/test")]
    Task<object> TestAsync([Body] object data);
}