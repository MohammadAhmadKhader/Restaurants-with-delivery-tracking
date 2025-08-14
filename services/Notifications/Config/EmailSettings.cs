using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Shared.Contracts.Attributes;

namespace Notifications.Config;

public class EmailSettings
{
    public const string sectionName = "EmailSettings";

    [Required(ErrorMessage = "Port is required.")]
    [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
    public int Port { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [Masked]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "FromEmail is required.")]
    [EmailAddress(ErrorMessage = "FromEmail must be a valid email address.")]
    public string FromEmail { get; set; } = null!;

    [Required(ErrorMessage = "UserName is required.")]
    [Masked]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "SmtpServer is required.")]
    [MinLength(3, ErrorMessage = "SmtpServer must be at least 3 characters long.")]
    public string SmtpServer { get; set; } = null!;

    [Required(ErrorMessage = "UseSSL flag is required.")]
    public bool UseSSL { get; set; }

    /// <summary>
    /// Some testing mailing services may throw error on using authentication, This flag is used to turn the authentication off.
    /// </summary>
    public bool UseAuth { get; set; } = true;

    [Required(ErrorMessage = "BatchSize is required.")]
    [Range(1, int.MaxValue)]
    public int BatchSize { get; set; } = 100;

    [Required(ErrorMessage = "TimeLimitInMilliseconds is required.")]
    [Range(1, int.MaxValue)]
    public int TimeLimitInMilliseconds { get; set; } = 3000;
}