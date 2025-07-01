using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Shared.Utils;
using StackExchange.Redis;

namespace Shared.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddKvStore(this IServiceCollection services, IConfiguration config, string connectionString = "Redis")
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var url = config.GetConnectionString(connectionString);
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Connection string 'Redis' is not configured");
            }

            return ConnectionMultiplexer.Connect(url);
        });

        return services;
    }

    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services, IConfiguration config, string connectionString = "DefaultConnection")
        where TContext : DbContext
    {
        var connStr = config.GetConnectionString(connectionString);
        if (string.IsNullOrWhiteSpace(connStr))
        {
            throw new ArgumentException("Connection string 'DefaultConnection' is not configured");
        }

        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connStr);
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