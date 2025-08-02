namespace Shared.Kafka;
public class KafkaSettings
{
    public string? BootstrapServers { get; set; }
    public string? Acks { get; set; }
    public int MessageTimeoutMs { get; set; }
    public int RequestTimeoutMs { get; set; }
}