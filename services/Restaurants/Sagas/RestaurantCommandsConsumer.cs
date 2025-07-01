using MassTransit;
using Restaurants.Contracts.Dtos;
using Restaurants.Services.IServices;
using Shared.Kafka;
namespace Restaurants.Sagas;

public class RestaurantCommandsConsumer(
    ILogger<RestaurantCommandsConsumer> logger,
    IRestaurantsService restaurantsService,
    ITopicProducer<RestaurantCreatedEvent> restaurantCreatedEventProducer) : 
    IConsumer<RestaurantCreateCommand>
{
    private readonly ILogger<RestaurantCommandsConsumer> _logger = logger;
    private readonly IRestaurantsService _restaurantsService = restaurantsService;
    private readonly ITopicProducer<RestaurantCreatedEvent> _restaurantCreatedEventProducer = restaurantCreatedEventProducer;

    //private readonly ITopicProducer<OwnerCreatingFailed> _ownerCreatingFailedProducer = ownerCreatingFailedProducer;

    public async Task Consume(ConsumeContext<RestaurantCreateCommand> ctx)
    {
        _logger.LogInformation("Received Restarant create command {@RestaurantCreateCommand}", ctx.Message);
        var dto = ctx.Message;

        var createRestDto = new RestaurantCreateDto(
            dto.Name,
            dto.Description ?? "",
            dto.Phone ?? "");

        var newRestaurant = await _restaurantsService.CreateAsync(createRestDto, ctx.Message.InvitationId, ctx.Message.OwnerId);

        await _restaurantCreatedEventProducer.Produce(new RestaurantCreatedEvent(ctx.Message.InvitationId, ctx.Message.OwnerId, newRestaurant.Name));
    }
}