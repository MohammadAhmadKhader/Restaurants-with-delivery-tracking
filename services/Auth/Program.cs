using Auth.Endpoints;
using Auth.Data;
using Auth.Extensions;
using Shared.Middlewares;
using Shared.Extensions;
using DotNetEnv;
using Shared.Redis;
using Shared.Health;
using Shared.Observability;
using Shared.Tenant;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Configuration.AddGlobalConfig();
builder.Services.AddControllers();
builder.Services.AddNamingPolicy();
builder.Services.AddAppAuthentication(config);
builder.Services.AddRedis(config);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config, "DefaultConnection");
builder.Services.AddServiceLogging(host, config);
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Services.AddAppProblemDetails();
builder.Services.AddHttpClientsDependenciesWithClientsServices(config);
builder.Services.AddKafkaHandlers(config);
builder.Host.ValidateScopes();
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Postgres, HealthChecksEnum.Redis, HealthChecksEnum.Kafka]);
builder.Services.AddTenantProvider();

// runs on certain flag
await builder.Services.UseRestaurantPermissionsSynchronizer();

var app = builder.Build();

app.UseSerilogRequestLoggingWithTraceId();
// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseAppLocalization();

// end points
app.MapAuthEndpoints();
app.MapUsersEndpoints();
app.MapRolesEndpoints();
app.MapAddressesEndpoints();

// others
app.EnsureDatabaseCreated<AppDbContext>();
app.AddHealthChecksEndpoints();

await app.SeedDatabaseAsync();

app.Run();
public partial class Program { }