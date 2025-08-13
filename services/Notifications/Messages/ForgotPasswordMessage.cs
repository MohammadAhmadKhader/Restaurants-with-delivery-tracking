namespace Notifications.Messages;

public class ForgotPasswordMessage : EmailMessage
{
    public string UserName { get; set; } = string.Empty;
}