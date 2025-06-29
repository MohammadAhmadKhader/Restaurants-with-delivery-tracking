using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories;
public class MenusRepository(AppDbContext ctx): GenericRepository<Menu, int>(ctx), IMenusRepository
{
    
}