using System.Data.Common;
using Locations.Data;
using Locations.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Locations.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;
    public ICurrentLocationsRepository CurrentLocationsRepository { get; } 
    public ILocationsHistoriesRepostiory LocationsHistoriesRepostiory { get; } 
    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        CurrentLocationsRepository = new CurrentLocationsRepository(_ctx);
        LocationsHistoriesRepostiory = new LocationsHistoriesRepository(_ctx);
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