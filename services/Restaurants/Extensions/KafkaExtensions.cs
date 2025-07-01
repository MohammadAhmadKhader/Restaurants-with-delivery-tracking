using Shared.Kafka;
using MassTransit;
using Restaurants.Sagas;
using StackExchange.Redis;
using Shared.Config;

namespace Restaurants.Extensions;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddMassTransitWithKafka<Program>((ctx, k)=>
        {
            var serviceName = "restaurant-service";
            k.TopicEndpoint<AcceptedInvitationEvent>(KafkaEventsTopics.InvitationAccepted, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantEventsConsumer>(ctx);
            });

            k.TopicEndpoint<OwnerCreatedEvent>(KafkaEventsTopics.RestaurantOwnerCreated, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantEventsConsumer>(ctx);
            });

            k.TopicEndpoint<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantEventsConsumer>(ctx);
            });

            k.TopicEndpoint<OwnerCreateCommand>(KafkaCommandsTopics.CreateRestaurantOwner, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCommandsConsumer>(ctx);
            });

            k.TopicEndpoint<RestaurantCreateCommand>(KafkaCommandsTopics.CreateRestaurant, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantCommandsConsumer>(ctx);
            });

            k.TopicEndpoint<SimpleTestEvent>(KafkaEventsTopics.TestTopic, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantEventsConsumer>(ctx);
            });

        }, (r) =>
        {
            // TODO: Redis Config must be an object with multiple key-values
            r.AddSagaStateMachine<RestaurantSaga, RestaurantCreateSagaData>();
            r.SetRedisSagaRepositoryProvider((r) =>
            {
                var redisConfig = config.GetRequiredSection("Redis").Get<string>()!;
                r.ConnectionFactory(() =>
                {
                    return ConnectionMultiplexer.Connect(new ConfigurationOptions
                    {
                        EndPoints = { redisConfig },
                    });
                });
            });
        });
        return services;
    }
}