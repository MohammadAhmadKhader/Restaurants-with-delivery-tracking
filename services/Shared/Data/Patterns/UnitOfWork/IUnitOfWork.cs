using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Shared.Data.Patterns.UnitOfWork;

public interface IUnitOfWork<TContext> 
    where TContext: DbContext
{
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}