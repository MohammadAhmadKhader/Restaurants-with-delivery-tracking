using System.Data.Common;

namespace Restaurants.Repositories.IRepositories;

public interface IUnitOfWork
{
    IRestaurantsRepository RestaurantsRepository { get; }
    IRestaurantInvitationsRepository RestaurantInvitationsRepository { get; }
    IMenusRepository MenusRepository { get; }
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}