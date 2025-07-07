using Auth.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories.IRepositories;
public interface IRestaurantPermissionsRepository : IGenericRepository<RestaurantPermission, int>
{

}