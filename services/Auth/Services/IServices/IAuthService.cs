using Auth.Dtos.Auth;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IAuthService
{
    Task<(User user, TokenResponse tokenData)> Register(RegisterDto dto);
    Task<(User user, TokenResponse tokenData)> Login(LoginDto dto);
    Task ChangePassword(Guid userId, ResetPasswordDto dto);
}