using Auth.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories.IRepositories;

public interface IRolesRepository : IGenericRepository<Role, Guid>
{
    Task<Role?> FindByNameAsync(string name);
    Task<Role?> FindByNameOrDisplayNameAsync(string name, string displayName);
    Task<Role?> FindByIdWithPermissionsAsync(Guid id);
    Task<Role?> FindByNameWithPermissionsAsync(string name);
}