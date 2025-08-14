using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Shared.Observability.Telemetry;
using Shared.Utils;

namespace Shared.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddNpgsqlDatabase<TContext>(
        this IServiceCollection services, IConfiguration config,
        string connectionString = "DefaultConnection",
        Action<DbContextOptionsBuilder>? optionsAction = null,
        Action<NpgsqlDbContextOptionsBuilder>? ctxOptionsBuilder = null,
        bool addTelemetryInterceptor = true)
        where TContext : DbContext
    {
        var connStr = config.GetConnectionString(connectionString);
        if (string.IsNullOrWhiteSpace(connStr))
        {
            throw new ArgumentException("Connection string 'DefaultConnection' is not configured");
        }

        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connStr, ctxOptionsBuilder);
            optionsAction?.Invoke(options);

            if (addTelemetryInterceptor)
            {
                options.AddInterceptors(new TelemetryDbInterceptor());
            }
        });

        return services;
    }

    /// <summary>
    /// this only must be used in early development and only works in development environment.
    /// </summary>
    public static WebApplication EnsureDatabaseCreated<TContext>(this WebApplication app)
        where TContext : DbContext
    {
        if (!EnvironmentUtils.IsDevelopment())
        {
            return app;
        }

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        db.Database.EnsureCreated();

        return app;
    }
}