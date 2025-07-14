using Auth.Endpoints;
using Auth.Data;
using Auth.Extensions;
using Shared.Middlewares;
using Shared.Extensions;
using DotNetEnv;
using Auth.Middlewares;
using Shared.Redis;
using Shared.Health;
using Shared.Observability;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;
var serviceName = "auth";

builder.Services.AddControllers();
builder.Services.AddNamingPolicy();
builder.Services.AddAppAuthentication(config);
builder.Services.AddRedis(config, serviceName);
builder.Services.AddDatabase<AppDbContext>(config, "DefaultConnection");
builder.Services.AddServiceLogging(host);
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Services.AddAppProblemDetails();
builder.Services.AddHttpClientsDependenciesWithClientsServices();
builder.Services.AddKafkaHandlers();
builder.Host.ValidateScopes();
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Postgres, HealthChecksEnum.Redis, HealthChecksEnum.Kafka]);

var app = builder.Build();

app.UseSerilogRequestLoggingWithTraceId();
// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseAppLocalization();

// auth
app.UseAuthentication();
app.UseAuthorization();

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