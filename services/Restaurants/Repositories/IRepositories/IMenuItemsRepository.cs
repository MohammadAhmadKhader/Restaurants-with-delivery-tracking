using Restaurants.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories.IRepositories;
public interface IMenuItemsRepository: IGenericRepository<MenuItem, int>
{
    
}