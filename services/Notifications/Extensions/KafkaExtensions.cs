using MassTransit;
using Microsoft.Extensions.Options;
using Notifications.Config;
using Notifications.Consumers;
using Notifications.Messages;
using Notifications.Utils;
using Shared.Kafka;
using Shared.Utils;

namespace Notifications.Extensions;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaHandlers(this IServiceCollection services, IConfiguration config)
    {
        if (EnvironmentUtils.IsOnlyKafkaRunInMemory())
        {
            services.AddMassTransitWithKafkaInMemory<Program>((ctx, cfg) =>
            {
                var emailSettings = ctx.GetRequiredService<IOptions<EmailSettings>>().Value;
                cfg.ReceiveEndpoint(nameof(RestaurantInvitationMessage), e =>
                {
                    e.ConfigureConsumer<EmailBatchConsumer>(ctx, configs =>
                    {
                        configs.Options<BatchOptions>(batch =>
                        {
                            batch.MessageLimit = emailSettings.BatchSize;
                            batch.TimeLimit = TimeSpan.FromMilliseconds(emailSettings.TimeLimitInMilliseconds);
                        });
                    });
                });

                cfg.ReceiveEndpoint(nameof(ForgotPasswordMessage), e =>
                {
                    e.ConfigureConsumer<EmailBatchConsumer>(ctx, configs =>
                    {
                        configs.Options<BatchOptions>(batch =>
                        {
                            batch.MessageLimit = emailSettings.BatchSize;
                            batch.TimeLimit = TimeSpan.FromMilliseconds(emailSettings.TimeLimitInMilliseconds);
                        });
                    });
                });
            });

            return services;
        }

        var serivceName = GeneralUtils.GetServiceName();
        services.AddMassTransitWithKafka<Program>(config, async (ctx, cfg) =>
        {
            var emailSettings = ctx.GetRequiredService<IOptions<EmailSettings>>().Value;
            var kafkaSettings = ctx.GetRequiredService<IOptions<KafkaSettings>>().Value;

            await KafkaTopicUtils.EnsureTopicExistsAsync(
                kafkaSettings.BootstrapServers!,
                BatchingTopics.BatchingEmailsTopic 
            );

            cfg.TopicEndpoint<EmailMessage>(BatchingTopics.BatchingEmailsTopic, serivceName, e =>
            {
                e.Batch<EmailMessage>(batch =>
                {
                    batch.MessageLimit = emailSettings.BatchSize;
                    batch.TimeLimit = TimeSpan.FromMilliseconds(emailSettings.TimeLimitInMilliseconds);
                });
                
                e.ConfigureConsumer<EmailBatchConsumer>(ctx);
            });
        }, (riderCfg) =>
        {
            riderCfg.AddProducer<RestaurantInvitationMessage>(BatchingTopics.BatchingEmailsTopic);
            riderCfg.AddProducer<ForgotPasswordMessage>(BatchingTopics.BatchingEmailsTopic);
        });

        return services;
    }
}