using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
}