using Shared.Kafka;
using MassTransit;
using Restaurants.Sagas;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Shared.Utils;

namespace Restaurants.Extensions;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services, IConfigurationRoot config)
    {
        var serviceName = GeneralUtils.GetServiceName();
        services.AddMassTransitWithKafka<Program>(config, (ctx, k) =>
        {
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