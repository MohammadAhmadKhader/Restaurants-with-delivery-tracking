using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories;

public class RestaurantPermissionsRepository(AppDbContext ctx) : GenericRepository<RestaurantPermission, int, AppDbContext>(ctx), IRestaurantPermissionsRepository
{
    
}