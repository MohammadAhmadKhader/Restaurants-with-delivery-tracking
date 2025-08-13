using System.ComponentModel.DataAnnotations;

namespace Notifications.Config;

/// <summary>
/// Those urls will redirect user from email -> frontend app to do the action.
/// </summary>
public class EmailUrlsSettings
{
    public const string sectionName = "EmailUrlsSettings";

    [Required(ErrorMessage = "RestaurantInvitationBaseUrl is required")]
    [Url(ErrorMessage = "RestaurantInvitationBaseUrl is invalid Url")]
    public string RestaurantInvitationBaseUrl { get; set; } = default!;

    [Required(ErrorMessage = "ForgotPasswordBaseUrl is required")]
    [Url(ErrorMessage = "ForgotPasswordBaseUrl is invalid Url")]
    public string ForgotPasswordBaseUrl { get; set; } = default!;
}