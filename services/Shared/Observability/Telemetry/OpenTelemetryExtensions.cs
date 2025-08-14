using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Constants;
using Shared.Observability.Telemetry.Settings;
using Shared.Utils;

namespace Shared.Observability.Telemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddServiceTelemetry<TProgram>(
        this IServiceCollection services,
        IConfiguration config,
        Action<ResourceBuilder>? configureResource = null,
        Action<TracerProviderBuilder>? configureTracing = null)
        where TProgram : class
    {
        var serviceName = GeneralUtils.GetServiceName();
        var serviceVersion = GeneralUtils.GetServiceVersion<TProgram>();
        GuardUtils.ThrowIfNull(serviceVersion);

        var telemetrySettings = config.GetSection(TelemetrySettings.SectionName).Get<TelemetrySettings>();
        GuardUtils.ThrowIfNull(telemetrySettings);

        services
        .AddOpenTelemetry()
        .ConfigureResource(resource =>
        {
            resource.AddService(serviceName, serviceVersion);
            resource.AddAttributes(new Dictionary<string, object>
            {
                { "environment", EnvironmentUtils.GetEnvName() }
            });

            configureResource?.Invoke(resource);
        })
        .WithTracing(tracing =>
        {
            tracing.SetSampler(CreateSampler(telemetrySettings.Tracing.Sampler));
            tracing.AddSource(Activities.DatabaseActivity);
            tracing.AddSource(Activities.ServicesActivity);
            tracing.AddSource(Activities.TestActivity);

            ConfigureInstrumentation(tracing, telemetrySettings.Tracing.Instrumentation);

            tracing.AddOtlpExporter(o =>
            {
                ConfigureOtlpExporter(o, telemetrySettings.Tracing.OtlpExporter);
            });

            configureTracing?.Invoke(tracing);
        });

        return services;
    }

    public static IServiceCollection AddTelemetrySettings(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<TelemetrySettings>()
            .Bind(config.GetSection(TelemetrySettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SamplerSettings>()
            .Bind(config.GetSection($"{TelemetrySettings.SectionName}:Tracing:Sampler"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<OtlpExporterSettings>()
            .Bind(config.GetSection($"{TelemetrySettings.SectionName}:Tracing:OtlpExporter"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<InstrumentationSettings>()
            .Bind(config.GetSection($"{TelemetrySettings.SectionName}:Tracing:Instrumentation"))
            .ValidateDataAnnotations()
            .ValidateOnStart();


        return services;
    }

    public static IServiceCollection DecorateWithTracing(this IServiceCollection services, List<Type> types)
    {
        var proxyGenerator = new ProxyGenerator();
        foreach (var type in types)
        {
            services.Decorate(type, (inner, sp) =>
            {
                var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(type, inner, new TracingInterceptor());
                return proxy;
            });
        }

        return services;
    }

    private static Sampler CreateSampler(SamplerSettings samplerSettings)
    {
        return samplerSettings.Type.ToLowerInvariant() switch
        {
            "alwayson" => new AlwaysOnSampler(),
            "alwaysoff" => new AlwaysOffSampler(),
            "traceidratio" => new TraceIdRatioBasedSampler(samplerSettings.Ratio),
            _ => new AlwaysOnSampler()
        };
    }

    private static OtlpExportProtocol ParseOtlpProtocol(string protocol)
    {
        return protocol.ToLowerInvariant() switch
        {
            "grpc" => OtlpExportProtocol.Grpc,
            "httpprotobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => OtlpExportProtocol.Grpc
        };
    }

    private static void ConfigureInstrumentation(TracerProviderBuilder tracing, InstrumentationSettings settings)
    {
        if (settings.EnableAspNetCore)
        {
            tracing.AddAspNetCoreInstrumentation(options =>
            {
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    activity.SetTag("tenant.id", request.Headers[CustomHeaders.TenantHeader].FirstOrDefault());
                };
            });
        }

        if (settings.EnableHttpClient)
        {
            tracing.AddHttpClientInstrumentation();
        }

        if (settings.EnableMassTransit)
        {
            tracing.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
        }
    }


    private static void ConfigureOtlpExporter(OtlpExporterOptions options, OtlpExporterSettings settings)
    {
        options.Endpoint = new Uri(settings.Endpoint);
        options.Protocol = ParseOtlpProtocol(settings.Protocol);
        options.TimeoutMilliseconds = settings.TimeoutMs;
    }
}