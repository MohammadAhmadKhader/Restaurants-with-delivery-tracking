namespace Notifications.Services.IServices;

public interface IEmailService
{
    Task SendRestaurantInvitation(Guid invitationId, string toEmail);
    Task SendForgotPassword(string toEmail, string token);
}