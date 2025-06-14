using System.Data.Common;
using Orders.Repositories.IRepositories;

namespace Orders.Repositories.IRepositories;

public interface IUnitOfWork
{
    IOrdersRepository OrdersRepository { get; }
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}