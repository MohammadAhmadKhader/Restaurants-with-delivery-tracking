namespace Shared.Observability.Telemetry.Settings;

public class InstrumentationSettings
{
    public bool EnableAspNetCore { get; set; } = true;
    public bool EnableHttpClient { get; set; } = true;
    public bool EnableMassTransit { get; set; } = true;
    public bool EnableEntityFramework { get; set; } = false;
}