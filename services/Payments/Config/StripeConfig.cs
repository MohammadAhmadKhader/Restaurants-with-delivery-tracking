using System.ComponentModel.DataAnnotations;

namespace Payments.Config;

public class StripeConfig
{
    public const string sectionName = "StripeConfig";
    [MaxLength(126)]
    [MinLength(10)]
    [Required]
    public string Secret { get; set; } = default!;
    
    [MaxLength(126)]
    [MinLength(10)]
    [Required]
    public string WebhookSecret { get; set; } = default!;

    [Url]
    [Required]
    public string SuccessUrl { get; set; } = default!;

    [Url]
    [Required]
    public string CancelUrl { get; set; } = default!;
}