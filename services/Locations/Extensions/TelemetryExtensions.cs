using Locations.Services.IServices;
using Shared.Observability.Telemetry;

namespace Locations.Extensions;
public static class TelemetryExtensions
{
    public static IServiceCollection AddServicesWithTelemetry(this IServiceCollection services)
    {
        List<Type> servicesTypes = [
            typeof(ILocationsService)
        ];

        services.DecorateWithTracing(servicesTypes);

        return services;
    }
}