using System.Data.Common;

namespace Locations.Repositories.IRepositories;

public interface IUnitOfWork
{
    ICurrentLocationsRepository CurrentLocationsRepository { get; }
    ILocationsHistoriesRepostiory LocationsHistoriesRepostiory { get; }
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}