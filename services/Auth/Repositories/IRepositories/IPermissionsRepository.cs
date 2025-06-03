using Auth.Models;

public interface IPermissionsRepository
{
    Task<Permission?> FindByIdAsync(int id);
    Task<List<Permission>> FindByIds(List<int> ids);
    Task<List<Permission>> FindAllAsync();
    Task<Permission> CreateAsync(Permission permission);
    Task<bool> ExistsByName(string name);
}