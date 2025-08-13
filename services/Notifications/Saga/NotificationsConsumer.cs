using MassTransit;
using Notifications.Services.IServices;
using Shared.Kafka;

namespace Notifications.Saga;

public class NotificationsConsumer(
    IEmailService emailService
) : IConsumer<RestaurantInvitationCreatedEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Consume(ConsumeContext<RestaurantInvitationCreatedEvent> context)
    {
        var msg = context.Message;
        await _emailService.SendRestaurantInvitation(msg.InvitationId, msg.Email);
    }
}