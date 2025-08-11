using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shared.Kafka;

public static class KafkaExtensions
{
    public static IServiceCollection AddMassTransitWithKafka<TProgram>(
        this IServiceCollection services,
        IConfigurationRoot config,
        Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>? configurer = null,
        Action<IRiderRegistrationConfigurator>? riderConfigurer = null)
    {
        if (EnvironmentUtils.ShouldIgnoreKafka())
        {
            return services;
        }

        if (EnvironmentUtils.IsOnlyKafkaRunInMemory() && EnvironmentUtils.IsProduction())
        {
            throw new InvalidOperationException("In memory must not be used in Production.");
        }

        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("KafkaConfig");

        var kafkaSection = config.GetRequiredSection("Kafka");
        var producerConfig = kafkaSection.Get<ProducerConfig>();
        GuardUtils.ThrowIfNull(producerConfig);

        var bootstrapServers = producerConfig.BootstrapServers;

        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers,
        };

        // this is needed internally or will throw an error
        services.Configure<ProducerConfig>(kafkaSection);

        // this is for the use inside the application
        services.Configure<KafkaSettings>(kafkaSection);

        if (!EnvironmentUtils.IsOnlyKafkaRunInMemory())
        {
            // AdminClientConfig is required for KafkaTopicsInitializer
            services.AddSingleton(adminConfig);
            services.AddHostedService<KafkaTopicsInitializer>();
        }

        services.AddMassTransit(busConfigurer =>
        {
            busConfigurer.UsingInMemory();

            if (EnvironmentUtils.IsOnlyKafkaRunInMemory())
            {
                services.AddSingleton(typeof(ITopicProducer<>), typeof(MockTopicProducer<>));
                logger.LogWarning("Kafka Running in memory mode");
                return;
            }

            busConfigurer.AddRider((r) =>
            {
                r.AddConsumers(typeof(TProgram).Assembly);
                riderConfigurer?.Invoke(r);

                r.AddProducer<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(producerConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(producerConfig.MessageTimeoutMs ?? 5000);
                });
                r.AddProducer<OwnerCreatedEvent>(KafkaEventsTopics.RestaurantOwnerCreated, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(producerConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(producerConfig.MessageTimeoutMs ?? 5000);
                });
                // r.AddProducer<RestaurantCreatedEvent>(KafkaEventsTopics.RestaurantCreated, (producerCtx, producerCfg) =>
                // {
                //     producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(kafkaConfig.RequestTimeoutMs ?? 5000);
                //     producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(kafkaConfig.MessageTimeoutMs ?? 5000);
                // });

                r.AddProducer<OwnerCreatingFailedEvent>(KafkaEventsTopics.RestaurantOwnerCreatingFailed, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(producerConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(producerConfig.MessageTimeoutMs ?? 5000);
                });

                r.AddProducer<OrderCheckoutCompleted>(KafkaEventsTopics.OrderCheckoutCompleteted, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(producerConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(producerConfig.MessageTimeoutMs ?? 5000);
                });

                r.AddProducer<SimpleTestEvent>(KafkaEventsTopics.TestTopic, (producerCtx, producerCfg) =>
                {
                    producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(producerConfig.RequestTimeoutMs ?? 5000);
                    producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(producerConfig.MessageTimeoutMs ?? 5000);
                });

                r.UsingKafka((ctx, cfg) =>
                {
                    logger.LogInformation("Kafka Bootstrap Servers: {BootstrapServers}", bootstrapServers);
                    cfg.Host(bootstrapServers);

                    configurer?.Invoke(ctx, cfg);
                    cfg.Acks = Acks.All;

                    cfg.ClientId = "FoodDelivery";
                });
            });
        });

        return services;
    }
}