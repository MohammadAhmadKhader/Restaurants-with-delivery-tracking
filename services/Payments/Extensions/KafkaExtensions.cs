using Shared.Kafka;
using Shared.Utils;

namespace Payments.Extensions;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfigurationRoot config)
    {
        var serviceName = GeneralUtils.GetServiceName();
        services.AddMassTransitWithKafka<Program>(config);
        return services;
    }
}