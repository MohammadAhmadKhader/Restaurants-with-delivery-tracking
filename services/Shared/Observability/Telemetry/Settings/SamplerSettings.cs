using System.ComponentModel.DataAnnotations;

namespace Shared.Observability.Telemetry.Settings;

public class SamplerSettings
{
    [Required]
    [AllowedValues("AlwaysOn", "AlwaysOff", "TraceIdRatio")]
    public string Type { get; set; } = "AlwaysOn";
    
    [Range(0.0, 1.0, ErrorMessage = "Ratio must be between 0.0 and 1.0")]
    public double Ratio { get; set; } = 1.0;
}