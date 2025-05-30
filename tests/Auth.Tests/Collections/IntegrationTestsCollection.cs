using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Auth.Data;
using Microsoft.Extensions.Logging;
using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Xunit.Abstractions;
using Meziantou.Extensions.Logging.Xunit;
using Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Auth.Tests.Collections;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{

}

public class IntegrationTestsFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> Factory { get; }
    private const string ConnectionEnvVar = "ConnectionStrings__DefaultConnection";
    private ILogger<IntegrationTestsFixture> _logger;
    public HttpClient Client { get; set; }
    private TestDataLoader _loader;
    public List<User> Users { get; set; }
    public List<Role> Roles { get; set; }
    public string TestPassword { get; set; } = "123456";
    
    public IntegrationTestsFixture()
    {
        Env.Load();
        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            builder.ConfigureServices(configSvcs =>
            {
                var appDbContextDescriptor = configSvcs.FirstOrDefault(desc => desc.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (appDbContextDescriptor == null)
                {
                    throw new InvalidOperationException("Service Descriptor for 'DbContextOptions<AppDbContext>' was not found");
                }

                var connStr = Environment.GetEnvironmentVariable(ConnectionEnvVar);
                if (string.IsNullOrWhiteSpace(connStr))
                {
                    var envs = Environment.GetEnvironmentVariables();
                    throw new InvalidOperationException("Connection environment variable for testing database was not set");
                }

                configSvcs.Remove(appDbContextDescriptor);
                configSvcs.AddDbContext<AppDbContext>(options => options.UseNpgsql(connStr));
            });
        });

        _logger = Factory.Services.GetRequiredService<ILogger<IntegrationTestsFixture>>();
        Client = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<AppDbContext>();
        if (db == null)
        {
            _logger.LogError("AppDbContext was received as null");
            return;
        }

        var passwordHasher = scope.ServiceProvider.GetService<IPasswordHasher<User>>();
        if (passwordHasher == null)
        {
            _logger.LogError("IPasswordHasher<User> was received as null");
            return;
        }

        _loader = new TestDataLoader(db, passwordHasher);

        await db.Database.EnsureCreatedAsync();
        var (users, roles) = await _loader.InitializeAsync();
        Roles = roles;
        Users = users;
    }

    public async Task DisposeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await _loader.CleanAsync(db);
        await Factory.DisposeAsync();
    }

    public HttpClient CreateClientWithTestOutput(ITestOutputHelper output)
    {
        var factory = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.Services.AddSingleton<ILoggerProvider>(new XUnitLoggerProvider(output));
            });
        });

        return factory.CreateClient();
    }

    public TService GetService<TService>() where TService : notnull
    {
        using var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TService>();
    }
}