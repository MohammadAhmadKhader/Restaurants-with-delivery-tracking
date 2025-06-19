using Auth.Data;
using Auth.Contracts.Dtos.User;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Patterns.GenericRepository;
using Shared.Specifications;

namespace Auth.Repositories;

public class UsersRepository(AppDbContext ctx) : GenericRepository<User, Guid>(ctx), IUsersRepository
{
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.NormalizedEmail == email.ToUpper());
    }

    public async Task<User?> FindByEmailWithRolesAndPermissionsAsync(string email)
    {
        return await _dbSet
        .Where(u => u.NormalizedEmail == email.ToUpper())
        .Include(u => u.Roles)
        .ThenInclude(r => r.Permissions)
        .FirstOrDefaultAsync();
    }

    public async Task<(List<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams)
    {
        var query = _dbSet.AsQueryable();
        var spec = new UsersFilterSpecification(filterParams);
        var result = await SpecificationEvaluator<User>.GetQuery(query, spec).ToListAsync();
        var count = await SpecificationEvaluator<User>.GetQuery(query, spec).CountAsync();

        return (result, count);
    }

    public async Task<User?> FindByIdWithRolesAsync(Guid id)
    {
        var user = await _dbSet
        .Include(u => u.Roles)
        .Where(u => u.Id == id)
        .FirstOrDefaultAsync();
        
        return user;
    }

    public async Task<User?> FindByIdWithRolesAndPermissionsAsync(Guid id)
    {
        var user = await _dbSet
        .Include(u => u.Roles)
        .ThenInclude(r => r.Permissions)
        .Where(u => u.Id == id)
        .FirstOrDefaultAsync();

        return user;
    }
}