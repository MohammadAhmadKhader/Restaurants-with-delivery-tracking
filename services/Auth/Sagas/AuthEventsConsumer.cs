using Auth.Services.IServices;
using MassTransit;
using Shared.Kafka;
namespace Auth.Sagas;

public class AuthEventsConsumer(
        ILogger<AuthEventsConsumer> logger,
        IAuthService authService,
        IUsersService usersService,
        ITopicProducer<OwnerCreatedEvent> ownerCreatedEventProducer) :
    IConsumer<InvitationAcceptedEvent>,
    IConsumer<RestaurantCreatingFailedEvent>
{
    private readonly ILogger<AuthEventsConsumer> _logger = logger;
    private readonly IAuthService _authService = authService;
    private readonly IUsersService _usersService = usersService;
    private readonly ITopicProducer<OwnerCreatedEvent> _ownerCreatedEventProducer = ownerCreatedEventProducer;

    public async Task Consume(ConsumeContext<InvitationAcceptedEvent> ctx)
    {
        var invId = ctx.Message.InvitationId;
        _logger.LogInformation("Invitation was accepted {@InvitationAcceptedEvent}", ctx.Message);

        var registerDto = ctx.Message.Register;
        var restaurantDto = ctx.Message.Restaurant;

        var (newUser, _) = await _authService.Register(registerDto);

        await _ownerCreatedEventProducer.Produce(new (invId, newUser.Id, restaurantDto));
    }

    public async Task Consume(ConsumeContext<RestaurantCreatingFailedEvent> ctx)
    {
        _logger.LogInformation("Restaurant creating was failed {@InvitationAcceptedEvent}", ctx.Message);
        await _usersService.CompensateOwnerCreationAsync(ctx.Message.OwnerId);
        _logger.LogInformation("Restaurant owner was compensated successfully");
    }
}