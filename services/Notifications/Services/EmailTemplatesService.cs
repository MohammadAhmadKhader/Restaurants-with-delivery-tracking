using Microsoft.Extensions.Options;
using Notifications.Config;
using Notifications.Services.IServices;
using Notifications.Utils;
using Shared.Contracts.Attributes;

namespace Notifications.Services;

public class EmailTemplatesService(IOptions<EmailUrlsSettings> options) : IEmailTemplatesService
{
    private const string RestaurantInvitationLinkPlaceHolder = "{{invitation_link}}";
    private const string ReceiverNamePlaceholder = "{{receiver_name}}";
    private const string ForgotPasswordLinkPlaceHolder = "{{reset_link}}";

    private readonly EmailUrlsSettings _emailUrlsSettings = options.Value;

    public string GetForgotPasswordTemplate([Masked] string userName, string token)
    {
        return TemplatesUtils.ForgotPasswordTemplate
            .Replace(ForgotPasswordLinkPlaceHolder, _emailUrlsSettings.ForgotPasswordBaseUrl + $"?token={token}")
            .Replace(ReceiverNamePlaceholder, userName);
    }

    public string GetRestaurantInvitationTemplate(Guid invitationId, [Masked] string ownerName)
    {
        return TemplatesUtils.RestaurantInvitationTemplate
            .Replace(RestaurantInvitationLinkPlaceHolder, _emailUrlsSettings.RestaurantInvitationBaseUrl + $"?invitationId={invitationId}")
            .Replace(ReceiverNamePlaceholder, ownerName);
    }
}