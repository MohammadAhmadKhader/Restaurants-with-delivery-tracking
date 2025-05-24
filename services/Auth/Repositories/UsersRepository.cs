using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

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
}