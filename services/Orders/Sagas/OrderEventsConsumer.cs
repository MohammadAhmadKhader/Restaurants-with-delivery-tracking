using MassTransit;
using Orders.Services.IServices;
using Shared.Kafka;

namespace Orders.Sagas;
public class OrderEventsConsumer(
        ILogger<OrderEventsConsumer> logger,
        IOrdersService ordersService
): IConsumer<OrderCheckoutCompleted>
{
    private readonly ILogger<OrderEventsConsumer> _logger = logger;
    private readonly IOrdersService _ordersService = ordersService;

    public async Task Consume(ConsumeContext<OrderCheckoutCompleted> context)
    {
        _logger.LogInformation("Processing kafka event '{Event}'.", KafkaEventsTopics.OrderCheckoutCompleteted);
        await _ordersService.MarkAsPayedAsync(context.Message.OrderId);
        _logger.LogInformation("Kafka event '{Event}' was processed successfully.", KafkaEventsTopics.OrderCheckoutCompleteted);
    }
}