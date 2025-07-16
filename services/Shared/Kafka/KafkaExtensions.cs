using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shared.Kafka;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaConfig(this IServiceCollection services, IConfigurationRoot config)
    {
        var kafkaSection = config.GetSection("Kafka");
        if (kafkaSection == null)
        {
            throw new ArgumentException("Kafka section was not found");
        }

        var kafkaConfig = kafkaSection.Get<Dictionary<string, string>>()!;
        var bootstrapServers = kafkaConfig["BootstrapServers"];
        ArgumentException.ThrowIfNullOrEmpty(bootstrapServers);

        KafkaMetadata.BootstrapServers = bootstrapServers;

        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers,
        };

        services.Configure<ProducerConfig>(kafkaSection);
        services.AddSingleton(adminConfig);

        return services;
    }

    public static IServiceCollection AddMassTransitWithKafka<TProgram>(
        this IServiceCollection services,
        IConfigurationRoot config,
        Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configurer,
        Action<IRiderRegistrationConfigurator>? riderConfigurer = null)
    {
        if (EnvironmentUtils.ShouldIgnoreKafka())
        {
            return services;
        }

        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("KafkaConfig");

        var kafkaConfig = config.GetRequiredSection("Kafka").Get<ProducerConfig>();
        GuardUtils.ThrowIfNull(kafkaConfig);
        
        var bootstrapServers = kafkaConfig.BootstrapServers;

        services.AddKafkaConfig(config);
        services.AddHostedService<KafkaTopicsInitializer>();

        services.AddMassTransit(busConfigurer =>
        {
            busConfigurer.SetKebabCaseEndpointNameFormatter();
            busConfigurer.UsingInMemory();

            busConfigurer.AddRider((r) =>
            {
                r.AddConsumers(typeof(TProgram).Assembly);
                riderConfigurer?.Invoke(r);

                r.AddProducer<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });
                r.AddProducer<OwnerCreatedEvent>(KafkaEventsTopics.RestaurantOwnerCreated, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                });
                // r.AddProducer<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, (producerCtx, producerCfg) =>
                // {
                //     producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                //     producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                // });

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