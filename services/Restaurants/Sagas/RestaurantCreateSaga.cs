using MassTransit;
using Shared.Kafka;

namespace Restaurants.Sagas;

public class RestaurantCreateSaga :
    IConsumer<AcceptedInvitationEvent>,
    IConsumer<OwnerCreatedEvent>,
    IConsumer<RestaurantCreatedEvent>,
    IConsumer<SimpleTestEvent>
{
    private readonly ILogger<RestaurantCreateSaga> _logger;
    private readonly ITopicProducer<OwnerCreateCommand> _ownerCreateProducer;
    private readonly ITopicProducer<RestaurantCreateCommand> _restaurantCreateProducer;
    public RestaurantCreateSaga(
        ILogger<RestaurantCreateSaga> logger,
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

        var register = ctx.Message.Register;

        await _ownerCreateProducer.Produce(new OwnerCreateCommand(
            ctx.Message.InvitationId,
            register.FirstName,
            register.LastName,
            register.Email,
            register.Password));
    }

    public async Task Consume(ConsumeContext<OwnerCreatedEvent> ctx)
    {
        _logger.LogInformation("Owner was created {@OwnerCreatedEvent}", ctx);

        var restaurant = ctx.Message.Restaurant;

        await _restaurantCreateProducer.Produce(new RestaurantCreateCommand(
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

    public Task Consume(ConsumeContext<SimpleTestEvent> ctx)
    {
        System.Console.WriteLine("----------------------------------------------------------");
        System.Console.WriteLine("----------------------------------------------------------");
        System.Console.WriteLine("----------------------------------------------------------");
        System.Console.WriteLine("----------------------------------------------------------");
        _logger.LogInformation("Simple Test Event was received {@SimpleTestEvent}", ctx.Message);
        return Task.CompletedTask;
    }
}