using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Auth.Data;
using Microsoft.Extensions.Logging;
using DotNetEnv;
using Xunit.Abstractions;
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Shared.Extensions;

namespace Auth.Tests.Collections;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{

}

public class IntegrationTestsFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> Factory { get; }
    public TestDataLoader Loader = default!;
    public readonly string TestPassword = TestDataLoader.TestPassword;
    private const string ConnectionEnvVar = "ConnectionStrings__DefaultConnection";
    private readonly ILogger<IntegrationTestsFixture> _logger;
    public IntegrationTestsFixture()
    {
        Env.Load();
        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ApplyDefaultConfigurations<AppDbContext>(ConnectionEnvVar);
        });

        _logger = Factory.Services.GetRequiredService<ILogger<IntegrationTestsFixture>>();
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

        Loader = new TestDataLoader(db, passwordHasher);

        await db.Database.EnsureCreatedAsync();
        await Loader.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await Loader.CleanAsync(db);
        await Factory.DisposeAsync();
    }

    public HttpClient CreateClientWithTestOutput(ITestOutputHelper output)
    {
        return Factory.EnableTestLoggingToXunit(output).CreateClient();
    }
    public User GetSuperAdmin()
    {
        return Loader.Users.First(u => u.Email == TestDataLoader.SuperAdminEmail);
    }

    public User GetAdmin()
    {
        return Loader.Users.First(u => u.Email == TestDataLoader.AdminEmail);
    }

    public User GetUser()
    {
        return Loader.Users.First(u => u.Email == TestDataLoader.UserEmail);
    }

    public User GetRandomUser()
    {
        return Loader.Users.First(
        u => u.Email != TestDataLoader.UserEmail &&
        u.Email != TestDataLoader.AdminEmail &&
        u.Email != TestDataLoader.SuperAdminEmail);
    }
}