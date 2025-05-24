using Auth.Dtos;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IUsersService
{
    Task<bool> ExistsByEmail(string email);
    Task<User?> FindById(Guid Id);
    Task<User?> FindByEmailWithRolesAndPermissions(string email);
}