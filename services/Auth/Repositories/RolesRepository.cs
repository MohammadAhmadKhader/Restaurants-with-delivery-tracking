using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;

namespace Auth.Repositories;

public class RolesRepository(AppDbContext ctx) : GenericRepository<Role, Guid>(ctx), IRolesRepository
{
    
}