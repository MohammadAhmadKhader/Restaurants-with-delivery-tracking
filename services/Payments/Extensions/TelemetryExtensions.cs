using Payments.Services.IServices;
using Shared.Observability.Telemetry;

namespace Payments.Extensions;
public static class TelemetryExtensions
{
    public static IServiceCollection AddServicesWithTelemetry(this IServiceCollection services)
    {
        List<Type> servicesTypes = [
            typeof(IPaymentsService), typeof(IStripeWebhookService)
        ];

        services.DecorateWithTracing(servicesTypes);

        return services;
    }
}