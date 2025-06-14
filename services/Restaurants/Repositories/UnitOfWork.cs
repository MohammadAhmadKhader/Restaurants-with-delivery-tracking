using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Restaurants.Data;
using Restaurants.Repositories.IRepositories;

namespace Restaurants.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;
    public IMenusRepository MenusRepository { get; set; }
    public IRestaurantsRepository RestaurantsRepository { get; set; }
    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        MenusRepository = new MenusRepository(_ctx);
        RestaurantsRepository = new RestaurantsRepository(_ctx);
    }
    public async Task<DbTransaction> BeginTransactionAsync()
    {
        var tx = await _ctx.Database.BeginTransactionAsync();
        return tx.GetDbTransaction();
    }

    public async Task CommitTransactionAsync(DbTransaction tx)
    {
        await _ctx.SaveChangesAsync();
        await tx.CommitAsync();
    }
    public async Task RollBackAsync(DbTransaction tx) => await tx.RollbackAsync();
    public async Task<int> SaveChangesAsync() => await _ctx.SaveChangesAsync();
}