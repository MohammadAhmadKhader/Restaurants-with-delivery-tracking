using DotNetEnv;
using Locations.Data;
using Locations.Endpoints;
using Locations.Extensions;
using Shared.Extensions;
using Shared.Middlewares;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddAppServiceDefaults<Program>(config, host);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config);
builder.Services.AddConventionalAppServices<Program, AppDbContext>();
builder.Services.AddServicesWithTelemetry();

var app = builder.Build();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

app.MapLocationsEndpoints();

app.Run();
public partial class Program { }
