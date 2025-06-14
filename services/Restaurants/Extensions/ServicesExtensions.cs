using Restaurants.Services;
using Restaurants.Services.IServices;

namespace Restaurants.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMenusService, MenusService>();
        services.AddScoped<IRestaurantsService, RestaurantsService>();

        return services;
    }
}