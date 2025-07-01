using MassTransit;
using Shared.Kafka;

namespace Restaurants.Sagas;

public class RestaurantEventsConsumer :
    IConsumer<AcceptedInvitationEvent>,
    IConsumer<OwnerCreatedEvent>,
    IConsumer<RestaurantCreatedEvent>
{
    private readonly ILogger<RestaurantEventsConsumer> _logger;
    private readonly ITopicProducer<OwnerCreateCommand> _ownerCreateProducer;
    private readonly ITopicProducer<RestaurantCreateCommand> _restaurantCreateProducer;
    public RestaurantEventsConsumer(
        ILogger<RestaurantEventsConsumer> logger,
        ITopicProducer<OwnerCreateCommand> ownerCreateProducer,
        ITopicProducer<RestaurantCreateCommand> restaurantCreateProducer)
    {
        _logger = logger;
        _ownerCreateProducer = ownerCreateProducer;
        _restaurantCreateProducer = restaurantCreateProducer;
    }

    public async Task Consume(ConsumeContext<AcceptedInvitationEvent> ctx)
    {
        _logger.LogInformation("Invitation was accepted {@AcceptedInvitationEvent}", ctx.Message);

        await _ownerCreateProducer.Produce(new OwnerCreateCommand(
            ctx.Message.InvitationId,
            ctx.Message.Register,
            ctx.Message.Restaurant));
    }

    public async Task Consume(ConsumeContext<OwnerCreatedEvent> ctx)
    {
        _logger.LogInformation("Owner was created {@OwnerCreatedEvent}", ctx);

        var restaurant = ctx.Message.Restaurant;

        await _restaurantCreateProducer.Produce(new RestaurantCreateCommand(
            ctx.Message.InvitationId,
            ctx.Message.OwnerId,
            restaurant.Name,
            restaurant.Description,
            restaurant.Phone
        ));
    }

    public Task Consume(ConsumeContext<RestaurantCreatedEvent> ctx)
    {
        _logger.LogInformation("Restaurant was created {@RestaurantCreatedEvent}", ctx.Message);
        return Task.CompletedTask;
    }
}