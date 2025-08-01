using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utils;

namespace Shared.Extensions;

public static class InfrastructureExtensions
{
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