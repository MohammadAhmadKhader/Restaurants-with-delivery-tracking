using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Orders.Data;
using Orders.Repositories.IRepositories;

namespace Orders.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;
    public IOrdersRepository OrdersRepository { get; }

    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        OrdersRepository = new OrdersRepository(_ctx);
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