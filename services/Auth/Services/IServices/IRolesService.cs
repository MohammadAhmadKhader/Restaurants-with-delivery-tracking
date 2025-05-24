using Auth.Models;

namespace Auth.Services.IServices;

public interface IRolesService
{
    Task<Role?> FindByIdAsync(Guid id);
    Task<Role?> FindByName(sbyte name);
}