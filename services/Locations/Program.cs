using DotNetEnv;
using Locations.Data;
using Locations.Endpoints;
using Shared.Extensions;
using Shared.Middlewares;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddAppServiceDefaults(config, host);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config);
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();

var app = builder.Build();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

app.MapLocationsEndpoints();

app.Run();
public partial class Program { }
