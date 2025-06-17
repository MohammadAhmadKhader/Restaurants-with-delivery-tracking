using Restaurants.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories.IRepositories;
public interface IRestaurantInvitationsRepository: IGenericRepository<RestaurantInvitation, Guid>
{

}