using System.Data.Common;

namespace Reviews.Repositories.IRepositories;

public interface IUnitOfWork
{
    IMenuItemReviewsRepository MenuItemReviewsRepository { get; set; }
    IRestaurantReviewRepository RestaurantReviewRepository { get; set; }
    Task<DbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(DbTransaction tx);
    Task RollBackAsync(DbTransaction tx);
    Task<int> SaveChangesAsync();
}