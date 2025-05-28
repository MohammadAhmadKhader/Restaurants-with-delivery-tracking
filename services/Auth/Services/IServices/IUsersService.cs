using Auth.Dtos;
using Auth.Dtos.User;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IUsersService
{
    Task<bool> ExistsByEmail(string email);
    Task<User?> FindById(Guid id);
    Task<User?> FindByIdWithRoles(Guid id);
    Task<User?> FindByIdWithRolesAndPermissions(Guid id);
    Task<User?> FindByEmailWithRolesAndPermissions(string email);
    Task<User?> UpdateProfile(Guid id, UserUpdateProfile dto);
    Task<(IReadOnlyList<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams);
    Task<(bool isSuccess, DeleteUserError error)> DeleteById(Guid id);
}