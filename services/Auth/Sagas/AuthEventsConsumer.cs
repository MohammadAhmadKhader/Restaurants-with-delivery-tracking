using Auth.Services.IServices;
using MassTransit;
using Shared.Kafka;
namespace Auth.Sagas;

public class AuthEventsConsumer(
        ILogger<AuthEventsConsumer> logger,
        IAuthService authService,
        ITopicProducer<OwnerCreatedEvent> ownerCreatedEventProducer) :
    IConsumer<RestaurantCreatedEvent>,
    IConsumer<SimpleTestEvent>
{
    private readonly ILogger<AuthEventsConsumer> _logger = logger;
    private readonly IAuthService _authService = authService;
    private readonly ITopicProducer<OwnerCreatedEvent> _ownerCreatedEventProducer = ownerCreatedEventProducer;

    public async Task Consume(ConsumeContext<RestaurantCreatedEvent> ctx)
    {
        var invId = ctx.Message.InvitationId;
        _logger.LogInformation("Restaurant was created with event {@RestaurantCreatedEvent}", ctx.Message);

        var registerDto = ctx.Message.Register;
        var restaurantId = ctx.Message.RestaurantId;
        var ownerId =  ctx.Message.OwnerId;

        var newUser = await _authService.CreateRestaurantOwnerAndRoles(registerDto, ownerId, restaurantId);

        await _ownerCreatedEventProducer.Produce(new(invId, newUser.Id, restaurantId));
    }

    public Task Consume(ConsumeContext<SimpleTestEvent> context)
    {
        _logger.LogInformation("Received event {@SimpleTestEvent}", context.Message);
        return Task.CompletedTask;
    }
}