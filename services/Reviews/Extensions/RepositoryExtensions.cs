using Reviews.Data;
using Reviews.Repositories;
using Reviews.Repositories.IRepositories;
using Shared.Data.Patterns.UnitOfWork;

namespace Reviews.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMenuItemReviewsRepository, MenuItemReviewsRepository>();
        services.AddScoped<IRestaurantReviewRepository, RestaurantReviewRepository>();
        services.AddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        
        return services;
    }
}