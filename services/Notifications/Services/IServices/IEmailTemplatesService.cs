namespace Notifications.Services.IServices;

public interface IEmailTemplatesService
{
    public string GetRestaurantInvitationTemplate(Guid invitationId, string ownerName);
    public string GetForgotPasswordTemplate(string userName, string token);
}