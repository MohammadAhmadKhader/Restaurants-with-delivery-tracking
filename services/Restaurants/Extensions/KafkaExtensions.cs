using Shared.Kafka;
using MassTransit;
using Restaurants.Sagas;

namespace Restaurants.Extensions;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services)
    {
        services.AddMassTransitWithKafka<Program>((ctx, k)=>
        {
            var serviceName = "restaurant-service";
            k.TopicEndpoint<AcceptedInvitationEvent>(KafkaEventsTopics.InvitationAccepted, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCreateSaga>(ctx);
            });

            k.TopicEndpoint<OwnerCreatedEvent>(KafkaEventsTopics.RestaurantOwnerCreated, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCreateSaga>(ctx);
            });

            k.TopicEndpoint<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCreateSaga>(ctx);
            });

            k.TopicEndpoint<OwnerCreateCommand>(KafkaCommandsTopics.CreateRestaurantOwner, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCreateHandler>(ctx);
            });

            k.TopicEndpoint<RestaurantCreateCommand>(KafkaCommandsTopics.CreateRestaurant, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCreateHandler>(ctx);
            });

            k.TopicEndpoint<SimpleTestEvent>(KafkaEventsTopics.TestTopic, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCreateSaga>(ctx);
            });
        
        });
        return services;
    }
}