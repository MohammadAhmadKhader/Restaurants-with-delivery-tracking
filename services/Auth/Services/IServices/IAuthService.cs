using Auth.Dtos;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IAuthService
{
    Task<(User user, TokenResponse tokenData)> Register(RegisterDto dto);
    Task<(User user, TokenResponse tokenData)> Login(LoginDto dto);
}