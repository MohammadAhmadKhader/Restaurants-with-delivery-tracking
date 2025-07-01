using Auth.Services.IServices;
using MassTransit;
using Shared.Kafka;
namespace Auth.Sagas;

public class AuthCommandsConsumer : 
    IConsumer<OwnerCreateCommand>
{
    private readonly ILogger<AuthCommandsConsumer> _logger;
    private readonly IAuthService _usersService;
    private readonly ITopicProducer<OwnerCreatedEvent> _ownerCreatedEventProducer;
    public AuthCommandsConsumer(ILogger<AuthCommandsConsumer> logger, IAuthService usersService, ITopicProducer<OwnerCreatedEvent> ownerCreatedEventProducer)
    {
        _logger = logger;
        _usersService = usersService;
        _ownerCreatedEventProducer = ownerCreatedEventProducer;
    }
    public async Task Consume(ConsumeContext<OwnerCreateCommand> ctx)
    {
        _logger.LogInformation("Invitation was accepted {@AcceptedInvitationEvent}", ctx.Message);

        var registerDto = ctx.Message.RegisterDto;
        var restaurantDto = ctx.Message.RestaurantCreateDto;

        var (newUser, _) = await _usersService.Register(registerDto);

        await _ownerCreatedEventProducer.Produce(new OwnerCreatedEvent(ctx.Message.InvitationId, newUser.Id, restaurantDto));
    }
}