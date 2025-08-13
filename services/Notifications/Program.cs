using DotNetEnv;
using Notifications.Data;
using Notifications.Endpoints;
using Notifications.Extensions;
using Shared.Extensions;
using Shared.Middlewares;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddAppServiceDefaults(config, host);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config);
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Services.AddConfigs(config);
builder.Services.AddEmailTemplatesService();
builder.Services.AddHttpClientsDependencies().AddRestaurantsClients(config);
builder.Services.AddKafkaHandlers(config);

var app = builder.Build();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

app.MapNotificationsEndpoints();

app.Run();
public partial class Program { }