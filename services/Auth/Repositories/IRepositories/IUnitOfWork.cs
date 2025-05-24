using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Auth.Repositories.IRepositories;

public interface IUnitOfWork
{
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}