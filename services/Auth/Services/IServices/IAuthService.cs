using Auth.Contracts.Dtos.Auth;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IAuthService
{
    Task<(User user, TokensResponse tokenData)> Register(RegisterDto dto);
    Task<(User user, TokensResponse tokenData)> RegisterRestaurant(RegisterDto dto, Guid restaurantId);
    Task<User> CreateRestaurantOwnerAndRoles(RegisterDto dto, Guid ownerId, Guid restaurantId);
    Task<(User user, TokensResponse tokenData)> Login(LoginDto dto);
    Task<(User user, TokensResponse tokenData)> LoginRestaurant(LoginDto dto);
    Task ChangePassword(Guid userId, ResetPasswordDto dto);
}