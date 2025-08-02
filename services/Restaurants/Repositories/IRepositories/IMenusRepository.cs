using Restaurants.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories.IRepositories;

public interface IMenusRepository : IGenericRepository<Menu, int>
{
    Task<Menu?> FindByNameAsync(string name);
    Task<Menu?> FindByIdWithItemsAsync(int id);
}