using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories;
public class RestaurantsRepository(AppDbContext ctx): GenericRepository<Restaurant, Guid>(ctx), IRestaurantsRepository
{
    
}