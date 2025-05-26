using Auth.Dtos.User;
using Auth.Models;

namespace Auth.Repositories.IRepositories;

public interface IUsersRepository : IGenericRepository<User, Guid>
{
    Task<bool> ExistsByEmail(string email);
    Task<User?> FindByEmailWithRolesAndPermissions(string email);
    Task<(IReadOnlyList<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams);
}