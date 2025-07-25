namespace Shared.Kafka;

public static class KafkaEventsTopics
{
    public const string RestaurantOwnerCreated = "events.restaurant-owner-created";
    public const string RestaurantOwnerCreatingFailed = "events.restaurant-owner-creating-failed";
    public const string RestaurantCreated = "events.restaurant-created";
    public const string RestaurantCreatingFailed = "events.restaurant-creating-failed";
    public const string TestTopic = "test-topic";
}