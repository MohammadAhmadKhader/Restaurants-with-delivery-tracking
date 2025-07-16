using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.OpenTelemetry;
using Serilog.Sinks.SystemConsole.Themes;
using Shared.Utils;

namespace Shared.Observability;

public static class LoggingExtensions
{
    public static IServiceCollection AddServiceLogging(
        this IServiceCollection services,
        ConfigureHostBuilder host,
        IConfigurationRoot config)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName();
        var serviceName = assemblyName?.Name ?? "UnknownService";

        var seqUrl = config.GetSection("Seq").Get<string>();
        ArgumentException.ThrowIfNullOrEmpty(seqUrl);

        host.UseSerilog((ctx, services, cfg) =>
        {
            cfg.ReadFrom.Configuration(config)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();

            var outputTemplate = config["Serilog:OutputTemplate"];
            ArgumentException.ThrowIfNullOrEmpty(outputTemplate);

            if (EnvironmentUtils.IsProduction())
            {
                cfg.WriteTo.Console(outputTemplate: outputTemplate);
            }
            else if (EnvironmentUtils.IsDevelopment() || EnvironmentUtils.IsTesting())
            {
                cfg.WriteTo.Console(
                    theme: AnsiConsoleTheme.Sixteen,
                    applyThemeToRedirectedOutput: true,
                    outputTemplate: outputTemplate
                );
            }

            if (!EnvironmentUtils.IsTesting())
            {
                cfg.WriteTo.Seq(seqUrl);
                cfg.Enrich.With(new ActivityEnricher())
                  .Enrich.WithProperty("ServiceName", serviceName);

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

            cfg.Filter.ByExcluding(Matching.WithProperty<string>("RequestPath", p => p.StartsWith("/health")));
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