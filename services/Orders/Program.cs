using DotNetEnv;
using Orders.Contracts.Enums;
using Orders.Data;
using Orders.Endpoints;
using Shared.Extensions;
using Shared.Middlewares;
using Shared.Observability;
using Shared.Tenant;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Configuration.AddGlobalConfig();
builder.Services.AddControllers();
builder.Services.AddNamingPolicy();
builder.Services.AddAppProblemDetails();
builder.Services.AddServiceLogging(host, config);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config, ctxOptionsBuilder: 
o => o.MapEnum<OrderStatus>(typeof(OrderStatus).Name.ToLower()));
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Services.AddTenantProvider();
builder.Services.AddHttpClientsDependenciesWithClientsServices(config);
builder.Host.ValidateScopes();

var app = builder.Build();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseAppLocalization();

app.EnsureDatabaseCreated<AppDbContext>();

// endpoints
app.MapOrdersEndpoints();

app.Run();
public partial class Program { }
