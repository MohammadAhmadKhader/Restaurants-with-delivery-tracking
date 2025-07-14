using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Utils;

namespace Shared.Observability;

public static class LoggingExtensions
{
    public static IServiceCollection AddServiceLogging(
        this IServiceCollection services,
        ConfigureHostBuilder host,
        IConfigurationRoot? serviceConfig = null,
        string? customServiceName = null)
    {
        var isTesting = EnvironmentUtils.IsTesting();
        var isDevelopment = EnvironmentUtils.IsDevelopment();
        var isProd = EnvironmentUtils.IsProduction();

        var config = serviceConfig ?? InternalUtils.GetSharedConfig();
        var seqUrl = config.GetSection("Seq").Get<string>();
        ArgumentException.ThrowIfNullOrEmpty(seqUrl);

        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
        var serviceName = customServiceName ?? assemblyName?.Name ?? "UnknownService";

        host.UseSerilog((ctx, services, cfg) =>
        {
            var outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] ({ServiceName}) {Message:lj}{NewLine}{Exception}";
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
                        applyThemeToRedirectedOutput: true,
                        outputTemplate: outputTemplate
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
                        applyThemeToRedirectedOutput: true,
                        outputTemplate: outputTemplate
                    );
            }

            if (!isTesting)
            {
                cfg.WriteTo.Seq(seqUrl);
                cfg.Enrich.WithProperty("ServiceName", serviceName);
                cfg.Enrich.With(new ActivityEnricher());

                // TODO: should be moved to a dedicated exporter
                cfg.WriteTo.OpenTelemetry(x =>
                {
                    x.Endpoint = seqUrl + "/ingest/otlp/v1/logs";
                    x.Protocol = OtlpProtocol.HttpProtobuf;
                    x.ResourceAttributes = new Dictionary<string, object>()
                    {
                        ["service.name"] = serviceName
                    };
                });
            }
        });
                
        return services;
    }

    public static IApplicationBuilder UseSerilogRequestLoggingWithTraceId(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                var activity = Activity.Current;
                if (activity != null)
                {
                    diagnosticContext.Set("TraceId", activity.TraceId.ToString());
                }
            };
        });

        return app;
    }
}