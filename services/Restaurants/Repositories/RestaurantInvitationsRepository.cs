using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories;
public class RestaurantInvitationsRepository(AppDbContext ctx): GenericRepository<RestaurantInvitation, Guid, AppDbContext>(ctx), IRestaurantInvitationsRepository
{
    
}