using Auth.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories.IRepositories;

public interface IRestaurantRolesRepository : IGenericRepository<RestaurantRole, Guid>
{
    Task<RestaurantRole?> FindByNameOrDisplayNameAsync(string name, string displayName);
    Task<RestaurantRole?> FindByIdWithPermissionsAsync(Guid id);
    Task<RestaurantRole?> FindByNameWithPermissionsAsync(string name);
    Task<RestaurantRole?> FindByNameAsync(string name);
}