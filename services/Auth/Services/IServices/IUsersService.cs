using Auth.Dtos;
using Auth.Dtos.User;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IUsersService
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> FindByIdAsync(Guid id);
    Task<User?> FindByIdWithRolesAsync(Guid id);
    Task<User?> FindByIdWithRolesAndPermissionsAsync(Guid id);
    Task<User?> FindByEmailWithRolesAndPermissionsAsync(string email);
    Task<User> UpdateProfileAsync(Guid id, UserUpdateProfile dto);
    Task<(List<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams);
    Task<(bool isSuccess, DeleteUserError error)> DeleteByIdAsync(Guid id);
}