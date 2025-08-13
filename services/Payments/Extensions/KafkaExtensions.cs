using Shared.Kafka;
using Shared.Utils;

namespace Payments.Extensions;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration config)
    {
        var serviceName = GeneralUtils.GetServiceName();
        if (EnvironmentUtils.IsOnlyKafkaRunInMemory())
        {
            services.AddMassTransitWithKafkaInMemory<Program>();
            return services;
        }

        services.AddMassTransitWithKafka<Program>(config);
        return services;
    }
}