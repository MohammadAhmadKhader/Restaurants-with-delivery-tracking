using DotNetEnv;
using Notifications.Endpoints;
using Notifications.Extensions;
using Shared.Extensions;
using Shared.Health;
using Shared.Middlewares;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddAppServiceDefaults<Program>(config, host);
builder.Services.AddConventionalAppServicesWithNoDatabase<Program>();
builder.Services.AddConfigs(config);
builder.Services.AddEmailTemplatesService();
builder.Services.AddHttpClientsDependencies().AddRestaurantsClients(config);
builder.Services.AddKafkaHandlers(config);
builder.Services.AddServicesWithTelemetry();
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Kafka]);

var app = builder.Build();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

app.MapNotificationsEndpoints();

app.AddHealthChecksEndpoints();

app.Run();
public partial class Program { }