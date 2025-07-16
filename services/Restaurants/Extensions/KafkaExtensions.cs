using Shared.Kafka;
using MassTransit;
using Restaurants.Sagas;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Restaurants.Extensions;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddMassTransitWithKafka<Program>(config, (ctx, k) =>
        {
            var serviceName = "restaurant-service";

            k.TopicEndpoint<OwnerCreatingFailedEvent>(KafkaEventsTopics.RestaurantOwnerCreatingFailed, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<RestaurantEventsConsumer>(ctx, c =>
                {
                    c.UseMessageRetry(r =>
                    {
                        r.Interval(7, TimeSpan.FromSeconds(10));
                    });

                    c.UseScheduledRedelivery(redeliver =>
                    {
                        redeliver.Handle<DbException>();
                        redeliver.Handle<DbUpdateException>();

                        redeliver.Intervals(
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromSeconds(30),
                            TimeSpan.FromMinutes(3)
                        );
                    });
                });
            });

        }, (rider) =>
        {
            rider.AddConsumeObserver<FailuresObserver>();
        });

        return services;
    }
}