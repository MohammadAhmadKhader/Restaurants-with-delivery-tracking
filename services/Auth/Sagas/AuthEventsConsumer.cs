using Auth.Data.Seed;
using Auth.Services.IServices;
using MassTransit;
using Shared.Kafka;
namespace Auth.Sagas;

public class AuthEventsConsumer(
        ILogger<AuthEventsConsumer> logger,
        IAuthService authService,
        ITopicProducer<OwnerCreatedEvent> ownerCreatedEventProducer) :
    IConsumer<InvitationAcceptedEvent>
{
    private readonly ILogger<AuthEventsConsumer> _logger = logger;
    private readonly IAuthService _authService = authService;
    private readonly ITopicProducer<OwnerCreatedEvent> _ownerCreatedEventProducer = ownerCreatedEventProducer;

    public async Task Consume(ConsumeContext<InvitationAcceptedEvent> ctx)
    {
        var invId = ctx.Message.InvitationId;
        _logger.LogInformation("Invitation was accepted {@InvitationAcceptedEvent}", ctx.Message);

        var registerDto = ctx.Message.Register;
        var restaurantId = ctx.Message.RestaurantId;
        var ownerId =  ctx.Message.OwnerId;

        var newUser = await _authService.CreateRestaurantOwnerAndRoles(registerDto, ownerId, restaurantId);

        await _ownerCreatedEventProducer.Produce(new(invId, newUser.Id, restaurantId));
    }
}