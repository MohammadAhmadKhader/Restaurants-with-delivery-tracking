using Auth.Sagas;
using MassTransit;
using Shared.Kafka;

namespace Auth.Extensions;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services)
    {
        var serviceName = "auth-service";
        services.AddMassTransitWithKafka<Program>((ctx, k) =>
        {
            k.TopicEndpoint<OwnerCreateCommand>(KafkaCommandsTopics.CreateRestaurantOwner, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<AuthCommandsConsumer>(ctx);
            });
        });

        return services;
    }
}