using Shared.Contracts.Attributes;

namespace Notifications.Messages;

public class ForgotPasswordMessage : EmailMessage
{
    [Masked]
    public string UserName { get; set; } = string.Empty;
}