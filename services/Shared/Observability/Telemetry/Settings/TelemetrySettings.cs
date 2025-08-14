using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Shared.Observability.Telemetry.Settings;

public class TelemetrySettings
{
    public const string SectionName = "Telemetry";
    
    [Required]
    [ValidateObjectMembers]
    public TracingSettings Tracing { get; set; } = new();
}

public class TracingSettings
{
    [Required]
    [ValidateObjectMembers]
    public SamplerSettings Sampler { get; set; } = new();

    [Required]
    [ValidateObjectMembers]
    public OtlpExporterSettings OtlpExporter { get; set; } = new();

    [Required]
    [ValidateObjectMembers]
    public InstrumentationSettings Instrumentation { get; set; } = new();
}