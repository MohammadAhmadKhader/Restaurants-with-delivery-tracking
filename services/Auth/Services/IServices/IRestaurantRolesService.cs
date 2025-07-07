using Auth.Contracts.Dtos.RestaurantRole;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IRestaurantRolesService
{
    Task<RestaurantRole?> FindByIdAsync(Guid id);
    Task<RestaurantRole?> FindByNameAsync(string name);
    Task<RestaurantRole?> FindByNameWithPermissionsAsync(string name);
    Task<RestaurantRole> CreateAsync(RestaurantRoleCreateDto dto);
    Task DeleteAsync(Guid id);
    Task<RestaurantRole> UpdateAsync(Guid id, RestaurantRoleUpdateDto dto);
    Task<RestaurantRole> AddPermissions(Guid roleId, List<int> permissionsIds);
    Task<RestaurantRole> RemovePermission(Guid roleId, int permissionId);
}
