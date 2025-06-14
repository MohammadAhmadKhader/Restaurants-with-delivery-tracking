using System.Data.Common;

namespace Payments.Repositories.IRepositories;

public interface IUnitOfWork
{
    IPaymentsRepository PaymentsRepository { get; }
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}