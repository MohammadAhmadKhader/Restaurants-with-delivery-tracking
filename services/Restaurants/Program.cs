using DotNetEnv;
using Restaurants.Data;
using Restaurants.Endpoints;
using Restaurants.Extensions;
using Shared.Extensions;
using Shared.Health;
using Shared.Middlewares;
using Shared.Redis;
using Shared.Observability;
using Shared.Storage.Cloudinary;
using Shared.Tenant;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddAppServiceDefaults<Program>(config, host);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config);
builder.Services.AddRedis(config);
builder.Services.AddConventionalAppServices<Program, AppDbContext>();
builder.Services.AddHttpClientsDependencies().AddAuthClients(config);
builder.Services.AddKafkaHandlers(config);
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Postgres, HealthChecksEnum.Redis, HealthChecksEnum.Kafka]);
builder.Services.AddCloudinaryStorage(config);
builder.Services.AddTenantProvider();
builder.Services.AddServicesWithTelemetry();

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