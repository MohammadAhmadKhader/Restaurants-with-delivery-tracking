using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Kafka;

public class KafkaTopicsInitializer : IHostedService
{
    private readonly static int TimeoutFetchMetadataInSeconds = 10;
    private readonly AdminClientConfig _adminConfig;
    private readonly ILogger<KafkaTopicsInitializer> _logger;
    public KafkaTopicsInitializer(AdminClientConfig adminConfig, ILogger<KafkaTopicsInitializer> logger)
    {
        _adminConfig = adminConfig;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var admin = new AdminClientBuilder(_adminConfig).Build();

        try
        {
            var allTopics = new[]
            {
                // events
                KafkaEventsTopics.RestaurantCreated,
                KafkaEventsTopics.RestaurantCreatingFailed,
                KafkaEventsTopics.RestaurantInvitationCreated,
                KafkaEventsTopics.RestaurantOwnerCreated,
                KafkaEventsTopics.RestaurantOwnerCreatingFailed,
                KafkaEventsTopics.OrderCheckoutCompleteted,
                KafkaEventsTopics.TestTopic,
            };

            var metadata = admin.GetMetadata(TimeSpan.FromSeconds(TimeoutFetchMetadataInSeconds));
            var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();
            var newTopicsSpecs = allTopics
                .Where(t => !existingTopics.Contains(t))
                .Select(t => new TopicSpecification
                {
                    Name = t,
                    NumPartitions = 1,
                    ReplicationFactor = 1,
                });

            var options = new CreateTopicsOptions { OperationTimeout = TimeSpan.FromSeconds(30) };
            if (newTopicsSpecs.Any())
            {
                var newTopicsNames = string.Join(", ", newTopicsSpecs);
                _logger.LogInformation($"Attempting to create the following topics: {newTopicsNames}");
                await admin.CreateTopicsAsync(newTopicsSpecs, options);
            }
            else
            {
                _logger.LogInformation("No new topics to create");
            }
            
        }
        catch (CreateTopicsException ex)
        {
            _logger.LogError("Error during attempt to create kafka topics {Message}.", ex.Error.Reason);
            throw;
        }
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}