using System.Data.Common;

namespace Auth.Repositories.IRepositories;

public interface IUnitOfWork
{
    IAddressesRepository AddressesRepository { get; }
    IUsersRepository UsersRepository { get; }
    IRolesRepository RolesRepository { get; }
    IPermissionsRepository PermissionsRepository { get; }
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}