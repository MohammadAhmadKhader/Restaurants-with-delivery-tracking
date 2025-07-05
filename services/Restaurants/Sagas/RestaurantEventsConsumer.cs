using MassTransit;
using Restaurants.Services.IServices;
using Shared.Kafka;

namespace Restaurants.Sagas;

public class RestaurantEventsConsumer(
    ILogger<RestaurantEventsConsumer> logger,
    ITopicProducer<RestaurantCreatedEvent> restaurantCreatedEventProducer,
    IRestaurantsService restaurantsService,
    IRestaurantInvitationsService restaurantInvitationsService) :
    IConsumer<OwnerCreatedEvent>,
    IConsumer<RestaurantCreatedEvent>,
    IConsumer<OwnerCreatingFailedEvent>,
    IConsumer<RestaurantCreatingFailedEvent>
{
    private readonly ILogger<RestaurantEventsConsumer> _logger = logger;
    private readonly ITopicProducer<RestaurantCreatedEvent> _restaurantCreatedEventProducer = restaurantCreatedEventProducer;
    private readonly IRestaurantsService _restaurantsService = restaurantsService;
    private readonly IRestaurantInvitationsService _restaurantInvitationsService = restaurantInvitationsService;

    public async Task Consume(ConsumeContext<OwnerCreatedEvent> ctx)
    {
        _logger.LogInformation("Owner was created {@OwnerCreatedEvent} starting to create restaurant", ctx.Message);
        var restaurant = ctx.Message.Restaurant;
        var newRestaurant = await _restaurantsService.CreateAsync(restaurant, ctx.Message.InvitationId, ctx.Message.OwnerId);

        await _restaurantCreatedEventProducer.Produce(
            new RestaurantCreatedEvent(
                ctx.Message.InvitationId,
                ctx.Message.OwnerId,
                newRestaurant.Id,
                newRestaurant.Name));
    }

    public Task Consume(ConsumeContext<RestaurantCreatedEvent> ctx)
    {
        _logger.LogInformation("Restaurant was created {@RestaurantCreatedEvent}", ctx.Message);
        return Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<OwnerCreatingFailedEvent> ctx)
    {
        _logger.LogInformation("Owner creating was failed with event {@OwnerCreatingFailedEvent}", ctx.Message);
        _logger.LogInformation("Compensating invitation marking, setting it as unused...");
        await _restaurantInvitationsService.CompensateMarkingInvitationAsUsedAsync(ctx.Message.InvitationId);
        _logger.LogInformation("Invitation marking was compensated successfully");
    }

    public async Task Consume(ConsumeContext<RestaurantCreatingFailedEvent> ctx)
    {
        _logger.LogInformation("Restauran creating was failed with event {@RestaurantCreatingFailedEvent}", ctx.Message);
        _logger.LogInformation("Compensating invitation marking, setting it as unused...");
        await _restaurantInvitationsService.CompensateMarkingInvitationAsUsedAsync(ctx.Message.InvitationId);
        _logger.LogInformation("Invitation marking was compensated successfully");
    }
}