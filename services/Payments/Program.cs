using DotNetEnv;
using Payments.Data;
using Payments.Endpoints;
using Payments.Extensions;
using Shared.Extensions;
using Shared.Health;
using Shared.Middlewares;
using Shared.Observability;
using Shared.Tenant;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddControllers();
builder.Configuration.AddGlobalConfig();
builder.Services.AddNamingPolicy();
builder.Services.AddAppProblemDetails();
builder.Services.AddServiceLogging(host, config);
builder.Services.AddServicesClientsWithDependencies(config);
builder.Services.AddNpgsqlDatabase<AppDbContext>(config);
builder.Services.AddTenantProvider();
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Host.ValidateScopes();
builder.Services.AddResourcesBatchRetrievers();
builder.Services.AddStripeConfig(config);
builder.Services.AddKafka(config);
builder.Services.AddAppHealthChecks(config, [HealthChecksEnum.Postgres, HealthChecksEnum.Kafka]);

var app = builder.Build();

app.UseSerilogRequestLoggingWithTraceId();
// middlewares
app.EnsureDatabaseCreated<AppDbContext>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();
app.UseAppLocalization();

app.AddHealthChecksEndpoints();

app.MapPaymentsEndpoints();
app.MapStripeWebhookEndpoints();

app.Run();
public partial class Program { }