using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Reviews.Data;
using Reviews.Repositories.IRepositories;

namespace Reviews.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;
    public IMenuItemReviewsRepository MenuItemReviewsRepository { get; set; }
    public IRestaurantReviewRepository RestaurantReviewRepository { get; set; }
    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        MenuItemReviewsRepository = new MenuItemReviewsRepository(_ctx);
        RestaurantReviewRepository = new RestaurantReviewRepository(_ctx);
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