using System.ComponentModel.DataAnnotations;


namespace Shared.Observability.Telemetry.Settings;

public class OtlpExporterSettings
{
    [Required]
    [Url(ErrorMessage = "Endpoint must be a valid URL")]
    public string Endpoint { get; set; } = "http://127.0.0.1:4317";

    [Required]
    [AllowedValues("Grpc", "HttpProtobuf")]
    public string Protocol { get; set; } = "Grpc";

    [Range(1000, 300000, ErrorMessage = "Timeout must be between 1000 and 300000 milliseconds")]
    public int TimeoutMs { get; set; } = 3000;
}