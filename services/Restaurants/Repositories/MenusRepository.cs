using Microsoft.EntityFrameworkCore;
using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Restaurants.Repositories;

public class MenusRepository(AppDbContext ctx) : GenericRepository<Menu, int>(ctx), IMenusRepository
{
    public async Task<Menu?> FindByIdWithItemsAsync(int id)
    {
        return await _dbSet.Where(x => x.Id == id)
                .Include(x => x.Items)
                .FirstOrDefaultAsync();
    }

    public async Task<Menu?> FindByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.NormalizedName == name.ToUpper());
    }
}