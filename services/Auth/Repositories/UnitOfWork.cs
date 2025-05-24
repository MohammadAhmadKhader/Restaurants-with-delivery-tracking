using System.Data.Common;
using Auth.Data;
using Auth.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Auth.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;
    public IAddressesRepository AddressesRepository { get; set; }
    public IUsersRepository UsersRepository { get; set; }
    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        AddressesRepository = new AddressesRepository(_ctx);
        UsersRepository = new UsersRepository(_ctx);
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