using Auth.Contracts.Dtos.Role;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IRolesService
{
    Task<Role?> FindByIdAsync(Guid id);
    Task<Role?> FindByNameAsync(string name);
    Task<(List<Role> roles, int count)> FindAllAsync(int page, int size);
    Task<Role> CreateAsync(RoleCreateDto dto);
    Task DeleteAsync(Guid id);
    Task<Role> UpdateAsync(Guid id, RoleUpdateDto dto);
    Task<Role> AddPermissions(Guid roleId, List<int> permissionsIds);
    Task<Role> RemovePermission(Guid roleId, int permissionId);
}