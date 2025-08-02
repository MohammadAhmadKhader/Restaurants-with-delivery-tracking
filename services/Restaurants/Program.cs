using DotNetEnv;
using Restaurants.Data;
using Restaurants.Endpoints;
using Restaurants.Extensions;
using Shared.Extensions;
using Shared.Health;
using Shared.Middlewares;
using Shared.Redis;
using Shared.Observability;
using Restaurants.Middlewares;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Configuration.AddGlobalConfig();
builder.Services.AddControllers();
builder.Services.AddNamingPolicy();
builder.Services.AddAppProblemDetails();
builder.Services.AddServiceLogging(host, config);
builder.Services.AddDatabase<AppDbContext>(config);
builder.Services.AddRedis(config, "restaurants");
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Services
.AddHttpClientsDependencies()
.AddAuthClients(config);

builder.Services.AddKafkaHandlers(config);
builder.Host.ValidateScopes();
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Postgres, HealthChecksEnum.Redis, HealthChecksEnum.Kafka]);

var app = builder.Build();

app.UseSerilogRequestLoggingWithTraceId();
// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseAppLocalization();

app.EnsureDatabaseCreated<AppDbContext>();
app.AddHealthChecksEndpoints();

// endpoints
app.MapRestaurantsEndpoints();
app.MapMenuItemsEndpoints();
app.MapMenusEndpoints();

app.Run();

public partial class Program { }