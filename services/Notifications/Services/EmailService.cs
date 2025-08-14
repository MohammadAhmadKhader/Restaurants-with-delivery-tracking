using Notifications.Services.IServices;
using Shared.Utils;
using MassTransit;
using Notifications.Enums;
using Notifications.Messages;
using Shared.Contracts.Attributes;

namespace Notifications.Services;

public class EmailService(
    IEmailTemplatesService templatesService,
    ITopicProducer<RestaurantInvitationMessage> restaurantInvitationProducer,
    ITopicProducer<ForgotPasswordMessage> forgotPasswordProducer
    ) : IEmailService
{
    private readonly IEmailTemplatesService _templatesService = templatesService;
    private readonly ITopicProducer<RestaurantInvitationMessage> _restaurantInvitationProducer = restaurantInvitationProducer;
    private readonly ITopicProducer<ForgotPasswordMessage> _forgotPasswordProducer = forgotPasswordProducer;

    public async Task SendRestaurantInvitation(Guid invitationId, [Masked] string toEmail)
    {
        var ownerName = GeneralUtils.CamelToPascal(toEmail.Split("@")[0]);
        var template = _templatesService.GetRestaurantInvitationTemplate(invitationId, ownerName);
    
        await _restaurantInvitationProducer.Produce(new RestaurantInvitationMessage
        {
            Subject = "Invitation to (RMS)",
            ToEmail = toEmail,
            TextBody = template,
            MessageType = MessageType.RestaurantInvitation,
            InvitationId = invitationId
        });
    }

    public async Task SendForgotPassword([Masked] string toEmail, string token)
    {
        var userName = GeneralUtils.CamelToPascal(toEmail.Split("@")[0]);
        var template = _templatesService.GetForgotPasswordTemplate(userName, token);

        await _forgotPasswordProducer.Produce(new ForgotPasswordMessage
        {
            Subject = "Forgot Password",
            ToEmail = toEmail,
            TextBody = template,
            MessageType = MessageType.ForgotPassword,
            UserName = userName
        });
    }
}