using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shared.Kafka;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaConfig(this IServiceCollection services)
    {
        var config = InternalUtils.GetSharedConfig();

        var kafkaConfig = config.GetRequiredSection("Kafka").Get<Dictionary<string, string>>()!;
        var BootstrapServers = kafkaConfig["BootstrapServers"];

        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = BootstrapServers,
        };

        services.Configure<ProducerConfig>(config.GetSection("Kafka"));
        services.AddSingleton(adminConfig);

        return services;
    }

    public static IServiceCollection AddMassTransitWithKafka<TProgram>(
        this IServiceCollection services, Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configurer,
        Action<IRiderRegistrationConfigurator>? riderConfigurer = null)
    {
        if (EnvironmentUtils.IsSeeding() || EnvironmentUtils.IsTesting())
        {
            return services;
        }

        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("KafkaConfig");

        var config = InternalUtils.GetSharedConfig();
        var kafkaConfig = config.GetRequiredSection("Kafka").Get<ProducerConfig>()!;
        var bootstrapServers = kafkaConfig.BootstrapServers;

        services.AddKafkaConfig();
        services.AddHostedService<KafkaTopicsInitializer>();

        services.AddMassTransit(busConfigurer =>
        {
            busConfigurer.SetKebabCaseEndpointNameFormatter();
            busConfigurer.UsingInMemory();

            busConfigurer.AddRider((r) =>
            {
                r.AddConsumers(typeof(TProgram).Assembly);
                riderConfigurer?.Invoke(r);

                r.AddProducer<InvitationAcceptedEvent>(KafkaEventsTopics.InvitationAccepted, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });
                r.AddProducer<OwnerCreatedEvent>(KafkaEventsTopics.RestaurantOwnerCreated, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });
                r.AddProducer<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });

                r.AddProducer<RestaurantCreatingFailedEvent>(KafkaEventsTopics.RestaurantCreatingFailed, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });

                r.AddProducer<OwnerCreatingFailedEvent>(KafkaEventsTopics.RestaurantOwnerCreatingFailed, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });

                r.AddProducer<SimpleTestEvent>(KafkaEventsTopics.TestTopic, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });

                r.UsingKafka((ctx, cfg) =>
                {
                    logger.LogInformation("Kafka Bootstrap Servers: {BootstrapServers}", bootstrapServers);
                    cfg.Host(bootstrapServers);

                    configurer(ctx, cfg);
                    cfg.Acks = Acks.All;

                    cfg.ClientId = "FoodDelivery";
                });
            });
        });

        return services;
    }
}