using Notifications.Services.IServices;
using Shared.Observability.Telemetry;

namespace Notifications.Extensions;
public static class TelemetryExtensions
{
    public static IServiceCollection AddServicesWithTelemetry(this IServiceCollection services)
    {
        List<Type> servicesTypes = [
            typeof(IEmailService), typeof(IEmailTemplatesService)
        ];

        services.DecorateWithTracing(servicesTypes);

        return services;
    }
}