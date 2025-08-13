using System.Data.Common;
using Auth.Sagas;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Kafka;
using Shared.Utils;

namespace Auth.Extensions;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services, IConfiguration config)
    {
        var serviceName = GeneralUtils.GetServiceName();
        if (EnvironmentUtils.IsOnlyKafkaRunInMemory())
        {
            services.AddMassTransitWithKafkaInMemory<Program>((ctx, cfg) =>
            {   
                cfg.ReceiveEndpoint(nameof(RestaurantCreatedEvent), e =>  e.ConfigureConsumer<AuthEventsConsumer>(ctx));
                cfg.ReceiveEndpoint(nameof(SimpleTestEvent), e => e.ConfigureConsumer<AuthEventsConsumer>(ctx));
            });

            return services;
        }

        services.AddMassTransitWithKafka<Program>(config, (ctx, k) =>
        {
            k.TopicEndpoint<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<AuthEventsConsumer>(ctx, c =>
                {
                    c.UseMessageRetry(r =>
                    {
                        r.Immediate(3);
                    });

                    c.UseScheduledRedelivery(redeliver =>
                    {
                        redeliver.Handle<DbException>();
                        redeliver.Handle<DbUpdateException>();

                        redeliver.Intervals(
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromSeconds(30),
                            TimeSpan.FromMinutes(5)
                        );
                    });
                });
            });

            k.TopicEndpoint<SimpleTestEvent>(KafkaEventsTopics.TestTopic, serviceName, cfg =>
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