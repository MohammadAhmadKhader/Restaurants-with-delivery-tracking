using Orders.Services.IServices;
using Shared.Observability.Telemetry;

namespace Orders.Extensions;
public static class TelemetryExtensions
{
    public static IServiceCollection AddServicesWithTelemetry(this IServiceCollection services)
    {
        List<Type> servicesTypes = [
            typeof(IOrdersService)
        ];

        services.DecorateWithTracing(servicesTypes);

        return services;
    }
}