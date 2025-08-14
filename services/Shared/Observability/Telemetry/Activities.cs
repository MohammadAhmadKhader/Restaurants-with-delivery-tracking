using Shared.Utils;

namespace Shared.Observability.Telemetry;

public static class Activities
{
    private static readonly string ServiceName = GeneralUtils.GetServiceName();
    public static readonly string DatabaseActivity = $"{ServiceName}.Database";
    public static readonly string ServicesActivity = $"{ServiceName}.Services";
    public static readonly string TestActivity = $"{ServiceName}.Test";
}