using DotNetEnv;
using Orders.Contracts.Enums;
using Orders.Data;
using Orders.Endpoints;
using Orders.Extensions;
using Shared.Extensions;
using Shared.Health;
using Shared.Middlewares;
using Shared.Observability;
using Shared.Observability.Telemetry;
using Shared.Tenant;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddAppServiceDefaults<Program>(config, host);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config, ctxOptionsBuilder:
o => o.MapEnum<OrderStatus>(typeof(OrderStatus).Name.ToLower()));
builder.Services.AddConventionalAppServices<Program, AppDbContext>();
builder.Services.AddTenantProvider();
builder.Services.AddServicesClientsWithDependencies(config);
builder.Services.AddResourcesBatchRetrievers();
builder.Services.AddKafkaHandlers(config);
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Postgres, HealthChecksEnum.Kafka]);
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
app.MapOrdersEndpoints();

app.Run();
public partial class Program { }
