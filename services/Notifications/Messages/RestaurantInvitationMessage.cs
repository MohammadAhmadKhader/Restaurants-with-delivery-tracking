namespace Notifications.Messages;

public class RestaurantInvitationMessage : EmailMessage
{
    public Guid InvitationId { get; set; }
}
