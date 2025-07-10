using MassTransit;
using Restaurants.Services.IServices;
using Shared.Kafka;

namespace Restaurants.Sagas;

public class RestaurantEventsConsumer(
    ILogger<RestaurantEventsConsumer> logger,
    IRestaurantInvitationsService restaurantInvitationsService) :
    IConsumer<OwnerCreatingFailedEvent>
{
    private readonly ILogger<RestaurantEventsConsumer> _logger = logger;
    private readonly IRestaurantInvitationsService _restaurantInvitationsService = restaurantInvitationsService;

    public async Task Consume(ConsumeContext<OwnerCreatingFailedEvent> ctx)
    {
        _logger.LogInformation("Owner creating was failed with event {@OwnerCreatingFailedEvent}", ctx.Message);
        _logger.LogInformation("Compensating invitation marking, setting it as unused...");
        await _restaurantInvitationsService.CompensateMarkingInvitationAsUsedAsync(ctx.Message.InvitationId);
        _logger.LogInformation("Invitation marking was compensated successfully");
    }
}