using System.Data.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Sagas;
using Shared.Kafka;
using Shared.Utils;

namespace Orders.Extensions;
public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services, IConfiguration config)
    {
        var serviceName = GeneralUtils.GetServiceName();
        if (EnvironmentUtils.IsOnlyKafkaRunInMemory())
        {
            services.AddMassTransitWithKafkaInMemory<Program>((ctx, cfg) =>
            {   
                cfg.ReceiveEndpoint(nameof(OrderCheckoutCompleted), e =>  e.ConfigureConsumer<OrderEventsConsumer>(ctx));
            });

            return services;
        }

        services.AddMassTransitWithKafka<Program>(config, (ctx, k) =>
        {
            k.TopicEndpoint<OrderCheckoutCompleted>(KafkaEventsTopics.OrderCheckoutCompleteted, serviceName, cfg =>
            {
                cfg.ConfigureConsumer<OrderEventsConsumer>(ctx, c =>
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

        }, (rider) =>
        {
            rider.AddConsumeObserver<FailuresObserver>();
        });
        return services;
    }
}