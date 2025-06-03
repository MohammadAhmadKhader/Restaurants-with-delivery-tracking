using Auth.Dtos.User;
using Auth.Models;

namespace Auth.Repositories.IRepositories;

public interface IUsersRepository : IGenericRepository<User, Guid>
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> FindByEmailWithRolesAndPermissionsAsync(string email);
    Task<User?> FindByIdWithRolesAsync(Guid id);
    Task<User?> FindByIdWithRolesAndPermissionsAsync(Guid id);
    Task<(IReadOnlyList<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams);
}