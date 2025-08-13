using Auth.Contracts.Dtos.User;
using Auth.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories.IRepositories;

public interface IUsersRepository : IGenericRepository<User, Guid>
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByEmailWithRolesAndPermissionsAsync(string email);
    Task<User?> FindByEmailWithRestaurantRolesAndPermissionsAsync(string email);
    Task<User?> FindByIdWithRolesAsync(Guid id);
    Task<User?> FindByIdWithRestaurantRolesAsync(Guid id);
    Task<User?> FindByIdWithRolesAndPermissionsAsync(Guid id);
    Task<User?> FindByIdWithRestaurantRolesAndPermissionsAsync(Guid id);
    Task<(List<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams);
}