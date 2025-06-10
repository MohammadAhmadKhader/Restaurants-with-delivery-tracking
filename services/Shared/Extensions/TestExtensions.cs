using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace Shared.Extensions;

public static class TestExtensions
{
    public static void ApplyDefaultConfigurations<TDbContext>(this IWebHostBuilder builder, string connectionEnvVar)
        where TDbContext : DbContext
    {
        builder.UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing").ConfigureServices(configSvcs =>
        {
            var appDbContextDescriptor = configSvcs.FirstOrDefault(desc => desc.ServiceType == typeof(DbContextOptions<TDbContext>));
            if (appDbContextDescriptor == null)
            {
                throw new InvalidOperationException("Service Descriptor for 'DbContextOptions<AppDbContext>' was not found");
            }

            var connStr = Environment.GetEnvironmentVariable(connectionEnvVar);
            if (string.IsNullOrWhiteSpace(connStr))
            {
                var envs = Environment.GetEnvironmentVariables();
                throw new InvalidOperationException("Connection environment variable for testing database was not set");
            }

            configSvcs.Remove(appDbContextDescriptor);
            configSvcs.AddDbContext<TDbContext>(options => options.UseNpgsql(connStr));
        });
    }

    public static WebApplicationFactory<TProgram> EnableTestLoggingToXunit<TProgram>(this WebApplicationFactory<TProgram> Factory, ITestOutputHelper output)
        where TProgram : class
    {
        var factory = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(output));
            });
        });

        return factory;
    }
}