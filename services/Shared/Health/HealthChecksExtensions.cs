using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Kafka;
using Shared.Utils;

namespace Shared.Health;

public static class HealthChecksExtensions
{
    private static readonly List<HealthChecksEnum> defaultHealthChecks = [HealthChecksEnum.Postgres];
    public static IServiceCollection AddAppHealthChecks
        (this IServiceCollection services,
        IConfiguration config,
        List<HealthChecksEnum>? healthChecks = null,
        string dbConnectionString = "DefaultConnection",
        string redisConnString = "Redis")
    {
        List<HealthChecksEnum> addedChecks = [];

        var healthCheckBuilder = services.AddHealthChecks();
        var dbUrl = config.GetConnectionString(dbConnectionString);
        var redisUrl = config.GetConnectionString(redisConnString);
        var bootstrapServers = KafkaMetadata.BootstrapServers;

        foreach (var healthCheck in healthChecks ?? defaultHealthChecks)
        {
            if (IsCheckNotAdded(addedChecks, healthCheck, HealthChecksEnum.Postgres))
            {
                ArgumentException.ThrowIfNullOrEmpty(dbUrl);
                healthCheckBuilder.AddNpgSql(dbUrl, tags: ["ready"]);
                addedChecks.Add(HealthChecksEnum.Postgres);
            }
            else if (IsCheckAdded(addedChecks, healthCheck, HealthChecksEnum.Postgres))
            {
                throw new InvalidOperationException(FormatErrorMessage(healthCheck));
            }

            if (IsCheckNotAdded(addedChecks, healthCheck, HealthChecksEnum.Redis))
            {
                ArgumentException.ThrowIfNullOrEmpty(redisUrl);
                healthCheckBuilder.AddRedis(redisUrl, tags: ["ready"]);
                addedChecks.Add(HealthChecksEnum.Redis);
            }
            else if (IsCheckAdded(addedChecks, healthCheck, HealthChecksEnum.Redis))
            {
                throw new InvalidOperationException(FormatErrorMessage(healthCheck));
            }

            if (healthCheck == HealthChecksEnum.Kafka && EnvironmentUtils.ShouldIgnoreKafka())
            {
                continue;
            }

            if (IsCheckNotAdded(addedChecks, healthCheck, HealthChecksEnum.Kafka))
            {
                ArgumentException.ThrowIfNullOrEmpty(bootstrapServers);
                healthCheckBuilder.AddKafka((setup) =>
                {
                    setup.BootstrapServers = bootstrapServers;
                }, tags: ["ready"]);
                addedChecks.Add(HealthChecksEnum.Kafka);
            }
            else if (IsCheckAdded(addedChecks, healthCheck, HealthChecksEnum.Kafka))
            {
                throw new InvalidOperationException(FormatErrorMessage(healthCheck));
            }

            if (healthCheck == HealthChecksEnum.Unknown)
            {
                throw new InvalidOperationException(FormatErrorMessage(healthCheck));
            }
        }

        healthCheckBuilder.AddCheck("live", () => HealthCheckResult.Healthy(), tags: [ "live" ]); ;

        return services;
    }

    public static WebApplication AddHealthChecksEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new()
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        app.MapHealthChecks("/health/ready", new()
        {
            Predicate = (check) => check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        return app;
    }

    private static string FormatErrorMessage(HealthChecksEnum check)
    {
        return $"Health Check {check} was already added";
    }

    private static bool IsCheckAdded(List<HealthChecksEnum> addedChecks, HealthChecksEnum healthCheck, HealthChecksEnum expectedHealthCheck)
    {
        return addedChecks.Contains(healthCheck) && healthCheck == expectedHealthCheck;
    }

    private static bool IsCheckNotAdded(List<HealthChecksEnum> addedChecks, HealthChecksEnum healthCheck, HealthChecksEnum expectedHealthCheck)
    {
        return !addedChecks.Contains(healthCheck) && healthCheck == expectedHealthCheck;
    }
}