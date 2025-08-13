using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Serilog;

namespace Shared.Kafka;
public static class KafkaTopicUtils
{
    public static async Task EnsureTopicExistsAsync(
        string bootstrapServers,
        string topicName,
        int numPartitions = 1,
        short replicationFactor = 1)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        };

        using var adminClient = new AdminClientBuilder(adminConfig).Build();

        try
        {
            var metadata = adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(5));
            bool topicExists = metadata.Topics.Any(t => t.Topic == topicName && t.Error.Code == ErrorCode.NoError);

            if (!topicExists)
            {
                var topicSpecification = new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                };

                await adminClient.CreateTopicsAsync([ topicSpecification ]);
                Log.Information($"Created topic: {topicName}");
            }
            else
            {
                Log.Information($"Topic {topicName} already exists.");
            }
        }
        catch (CreateTopicsException ex)
        {
            Log.Error(ex, $"An error occured creating topic {topicName}: {ex.Results[0].Error.Reason}");
            throw;
        }
    }
}