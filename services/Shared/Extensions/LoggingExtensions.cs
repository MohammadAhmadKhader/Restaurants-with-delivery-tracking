using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Shared.Extensions;
public static class LoggingExtensions
{
    public static IServiceCollection AddServiceLogging(this IServiceCollection services, ConfigureHostBuilder host)
    {
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
        .WriteTo.Console(
            theme: AnsiConsoleTheme.Sixteen,
            applyThemeToRedirectedOutput: true
        )
        .CreateLogger();

        var isTestingEnv = !AppDomain.CurrentDomain.FriendlyName.Contains("testhost", StringComparison.OrdinalIgnoreCase);
        if (isTestingEnv)
        {
            host.UseSerilog();
        }

        return services;
    }
}