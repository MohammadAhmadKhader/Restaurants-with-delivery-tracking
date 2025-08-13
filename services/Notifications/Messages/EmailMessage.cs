using Notifications.Enums;

namespace Notifications.Messages;

public class EmailMessage
{
    public string Subject { get; set; } = string.Empty;
    public string ToEmail { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
    public MessageType MessageType { get; set; } = MessageType.Unknown;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}