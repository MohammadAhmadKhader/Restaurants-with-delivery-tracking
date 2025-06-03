using Auth.Models;

namespace Auth.Repositories.IRepositories;

public interface IRolesRepository : IGenericRepository<Role, Guid>
{
    Task<Role?> FindByNameAsync(string name);
    Task<(List<Role> roles, int count)> FindAllAsync(int page, int size);
    Task<Role?> FindByNameOrDisplayNameAsync(string name, string displayName);
    Task<Role?> FindByIdWithPermissionsAsync(Guid id);
    Task<Role?> FindByNameWithPermissionsAsync(string name);
}