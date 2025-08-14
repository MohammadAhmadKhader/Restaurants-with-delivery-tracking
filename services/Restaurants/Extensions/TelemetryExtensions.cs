using Restaurants.Services.IServices;
using Shared.Observability.Telemetry;

namespace Restaurants.Extensions;
public static class TelemetryExtensions
{
    public static IServiceCollection AddServicesWithTelemetry(this IServiceCollection services)
    {
        List<Type> servicesTypes = [
            typeof(IMenusService), typeof(IRestaurantsService), typeof(IRestaurantInvitationsService)
        ];

        services.DecorateWithTracing(servicesTypes);

        return services;
    }
}