using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories;

public class RolesRepository(AppDbContext ctx) : GenericRepository<Role, Guid>(ctx), IRolesRepository
{
    public async Task<Role?> FindByNameAsync(string name)
    {
        return await _dbSet.Where(x => x.NormalizedName == name.ToUpper()).FirstOrDefaultAsync();
    }

    public async Task<Role?> FindByIdWithPermissionsAsync(Guid id)
    {
        return await _dbSet.Where(x => x.Id == id)
        .Include(x => x.Permissions)
        .FirstOrDefaultAsync();
    }

    public async Task<Role?> FindByNameOrDisplayNameAsync(string name, string displayName)
    {
        return await _dbSet.Where(
            x => x.NormalizedName == name.ToUpper() ||
            x.DisplayName == displayName
        ).FirstOrDefaultAsync();
    }

    public async Task<Role?> FindByNameWithPermissionsAsync(string name)
    {
        return await _dbSet
        .Include(x => x.Permissions)
        .Where(x => x.NormalizedName == name.ToUpper())
        .FirstOrDefaultAsync();
    }
}