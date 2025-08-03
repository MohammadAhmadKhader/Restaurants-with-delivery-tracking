using System.ComponentModel.DataAnnotations;

namespace Shared.Storage.Cloudinary;
public class CloudinarySettings
{
    [Required(ErrorMessage = "Cloudinary ApiKey is required.")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "ApiKey must be between {2} and {1} characters.")]
    public string ApiKey { get; set; } = default!;

    [Required(ErrorMessage = "Cloudinary ApiSecret is required.")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "ApiSecret must be between {2} and {1} characters.")]
    public string ApiSecret { get; set; } = default!;

    [Required(ErrorMessage = "Cloudinary CloudName is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "CloudName must be between {2} and {1} characters.")]
    public string CloudName { get; set; } = default!;
}