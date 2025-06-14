using Reviews.Repositories;
using Reviews.Repositories.IRepositories;

namespace Reviews.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMenuItemReviewsRepository, MenuItemReviewsRepository>();
        services.AddScoped<IRestaurantReviewRepository, RestaurantReviewRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}