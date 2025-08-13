using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shared.Kafka;

public static class KafkaExtensions
{
    public static Action<IRiderRegistrationContext, IKafkaProducerConfigurator<Null, TMessage>>
        GetDefaultProducerConfig<TMessage>(ProducerConfig producerConfig)
    where TMessage : class
    {
        return (producerCtx, producerCfg) =>
        {
            producerCfg.RequestTimeout = TimeSpan.FromMilliseconds(producerConfig.RequestTimeoutMs ?? 5000);
            producerCfg.MessageTimeout = TimeSpan.FromMilliseconds(producerConfig.MessageTimeoutMs ?? 5000);
        };
    }

    public static IServiceCollection AddMassTransitWithKafka<TProgram>(
        this IServiceCollection services,
        IConfiguration config,
        Action<IRiderRegistrationContext, IKafkaFactoryConfigurator>? configurer = null,
        Action<IRiderRegistrationConfigurator>? riderConfigurer = null)
    {
        if (EnvironmentUtils.ShouldIgnoreKafka())
        {
            return services;
        }

        var logger = GeneralUtils.GetLogger(services);

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

        // AdminClientConfig is required for KafkaTopicsInitializer
        services.AddSingleton(adminConfig);
        services.AddHostedService<KafkaTopicsInitializer>();

        services.AddMassTransit(busConfigurer =>
        {
            busConfigurer.UsingInMemory();
            busConfigurer.AddRider((r) =>
            {
                r.AddConsumers(typeof(TProgram).Assembly);
                riderConfigurer?.Invoke(r);

                r.AddProducer(KafkaEventsTopics.RestaurantCreated, GetDefaultProducerConfig<RestaurantCreatedEvent>(producerConfig));
                r.AddProducer(KafkaEventsTopics.RestaurantInvitationCreated, GetDefaultProducerConfig<RestaurantInvitationCreatedEvent>(producerConfig));
                r.AddProducer(KafkaEventsTopics.RestaurantOwnerCreated, GetDefaultProducerConfig<OwnerCreatedEvent>(producerConfig));
                r.AddProducer(KafkaEventsTopics.RestaurantOwnerCreatingFailed, GetDefaultProducerConfig<OwnerCreatingFailedEvent>(producerConfig));
                r.AddProducer(KafkaEventsTopics.OrderCheckoutCompleteted, GetDefaultProducerConfig<OrderCheckoutCompleted>(producerConfig));
                r.AddProducer(KafkaEventsTopics.TestTopic, GetDefaultProducerConfig<SimpleTestEvent>(producerConfig));

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

    public static IServiceCollection AddMassTransitWithKafkaInMemory<TProgram>(
        this IServiceCollection services,
        Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator>? inMemoryConfigurator = null
    )
    {
        var logger = GeneralUtils.GetLogger(services);
        logger.LogWarning("Kafka running in memory mode.");

        services.AddMassTransit(busConfigurer =>
        {
            busConfigurer.AddConsumers(typeof(TProgram).Assembly);
            services.AddScoped(typeof(ITopicProducer<>), typeof(TopicProducerInMemoryOverride<>));

            busConfigurer.UsingInMemory((ctx, cfg) =>
            {
                cfg.ConfigureEndpoints(ctx);
                inMemoryConfigurator?.Invoke(ctx, cfg);
            });
        });

        return services;
    }
}