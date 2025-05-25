using System.Data.Common;

namespace Auth.Repositories.IRepositories;

public interface IUnitOfWork
{
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}