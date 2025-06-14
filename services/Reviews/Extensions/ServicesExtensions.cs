using Reviews.Services;
using Reviews.Services.IServices;

namespace Reviews.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMenuItemReviewsService, MenuItemReviewsService>();
        services.AddScoped<IRestaurantReviewService, RestaurantReviewService>();

        return services;
    }
}