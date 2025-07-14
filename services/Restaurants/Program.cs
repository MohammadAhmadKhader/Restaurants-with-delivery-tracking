using DotNetEnv;
using Restaurants.Data;
using Restaurants.Endpoints;
using Restaurants.Extensions;
using Shared.Extensions;
using Shared.Health;
using Shared.Middlewares;
using Shared.Redis;
using Shared.Observability;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddControllers();
builder.Services.AddNamingPolicy();
builder.Services.AddAppProblemDetails();
builder.Services.AddServiceLogging(host);
builder.Services.AddDatabase<AppDbContext>(config);
builder.Services.AddRedis(config, "restaurants");
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Services
.AddHttpClientsDependencies()
.AddAuthClients();

builder.Services.AddKafkaHandlers(config);
builder.Host.ValidateScopes();
builder.Services.AddHealthChecks();
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Postgres, HealthChecksEnum.Redis, HealthChecksEnum.Kafka]);

var app = builder.Build();

app.UseSerilogRequestLoggingWithTraceId();
// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

app.MapRestaurantsEndpoints();
app.EnsureDatabaseCreated<AppDbContext>();
app.AddHealthChecksEndpoints();

app.Run();

public partial class Program { }