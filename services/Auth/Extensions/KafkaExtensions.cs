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
            k.TopicEndpoint<InvitationAcceptedEvent>(KafkaEventsTopics.InvitationAccepted, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<AuthEventsConsumer>(ctx);

            });

            k.TopicEndpoint<RestaurantCreatingFailedEvent>(KafkaEventsTopics.RestaurantCreatingFailed, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<AuthEventsConsumer>(ctx);
            });
        }, (rider) =>
        {
            rider.AddConsumeObserver<FailuresObserver>();
        });

        return services;
    }
}