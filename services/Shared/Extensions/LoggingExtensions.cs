using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Utils;

namespace Shared.Extensions;
public static class LoggingExtensions
{
    public static IServiceCollection AddServiceLogging(this IServiceCollection services, ConfigureHostBuilder host)
    {
        var isTesting = EnvironmentUtils.IsTesting();
        var isDevelopment = EnvironmentUtils.IsDevelopment();
        var isProd = EnvironmentUtils.IsProduction();
       
        host.UseSerilog((ctx, services, cfg) =>
        {
            if (isProd || isTesting)
            {
                cfg.MinimumLevel.Warning()
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

                if (isTesting)
                {
                    cfg.WriteTo.Console(
                        theme: AnsiConsoleTheme.Sixteen,
                        applyThemeToRedirectedOutput: true
                    );
                }
                else
                {
                    cfg.WriteTo.Console();
                }
            }
            else if (isDevelopment)
            {
                cfg.MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Information)
                    // we set this to warning this to avoid Linq very long logging
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                    .WriteTo.Console(
                        theme: AnsiConsoleTheme.Sixteen,
                        applyThemeToRedirectedOutput: true
                    );
            }
        });

        return services;
    }
}