using DotNetEnv;
using Restaurants.Data;
using Restaurants.Endpoints;
using Restaurants.Extensions;
using Shared.Extensions;
using Shared.Middlewares;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddControllers();
builder.Services.AddNamingPolicy();
builder.Services.AddAppProblemDetails();
builder.Services.AddServiceLogging(host);
builder.Services.AddDatabase<AppDbContext>(config);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services
.AddHttpClientsDependencies()
.AddAuthClients();

builder.Services.AddKafkaHandlers();
builder.Host.ValidateScopes();

var app = builder.Build();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

app.MapRestaurantsEndpoints();

app.Run();

public partial class Program { }