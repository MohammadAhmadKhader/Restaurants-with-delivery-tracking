using Microsoft.Extensions.Configuration;

namespace Shared.Utils;
internal class InternalUtils
{
    public static IConfigurationRoot GetSharedConfig()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"globalsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"globalsettings.{env}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}