using Auth.Data;
using Auth.Dtos.User;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Specifications;
using Microsoft.EntityFrameworkCore;
using Shared.Specifications;

namespace Auth.Repositories;

public class UsersRepository(AppDbContext ctx) : GenericRepository<User, Guid>(ctx), IUsersRepository
{
    public async Task<bool> ExistsByEmail(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email.ToLower());
    }

    public async Task<User?> FindByEmailWithRolesAndPermissions(string email)
    {
        return await _dbSet
        .Where(u => u.NormalizedEmail == email.ToUpper())
        .Include(u => u.Roles)
        .ThenInclude(r => r.Permissions)
        .FirstOrDefaultAsync();
    }
    
    public async Task<(IReadOnlyList<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams)
    {
        var query = _dbSet.AsQueryable();
        var spec = new UsersFilterSpecification(filterParams);
        var result = await SpecificationEvaluator<User>.GetQuery(query ,spec).ToListAsync();
        var count = await SpecificationEvaluator<User>.GetQuery(query ,spec).CountAsync();

        return (result, count);
    }
}