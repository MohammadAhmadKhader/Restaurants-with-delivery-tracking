using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories;

public class MenuItemsRepository(AppDbContext ctx) : GenericRepository<MenuItem, int, AppDbContext>(ctx), IMenuItemsRepository
{

}