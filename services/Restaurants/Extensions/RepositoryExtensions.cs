using Restaurants.Data;
using Restaurants.Repositories;
using Restaurants.Repositories.IRepositories;
using Shared.Data.Patterns.UnitOfWork;

namespace Restaurants.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMenusRepository, MenusRepository>();
        services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
        services.AddScoped<IRestaurantInvitationsRepository, RestaurantInvitationsRepository>();
        services.AddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        
        return services;
    }
}