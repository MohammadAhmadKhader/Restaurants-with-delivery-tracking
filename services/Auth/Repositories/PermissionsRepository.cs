
using Auth.Data;
using Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repositories;

public class PermissionsRepository(AppDbContext ctx) : IPermissionsRepository
{
    private readonly DbSet<Permission> _dbSet = ctx.Set<Permission>();
    private readonly AppDbContext _ctx = ctx;

    public async Task<Permission> CreateAsync(Permission permission)
    {
        var res = await _dbSet.AddAsync(permission);
        await _ctx.SaveChangesAsync();
        return res.Entity;
    }

    public async Task<bool> ExistsByName(string name)
    {
        return await _dbSet.AnyAsync(p => p.Name == name);
    }

    public async Task<List<Permission>> FindAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<Permission?> FindByIdAsync(int id)
    {
        return await _dbSet.Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Permission>> FindByIds(List<int> ids)
    {
        return await _dbSet.Where(p => ids.Contains(p.Id)).ToListAsync();
    }
}