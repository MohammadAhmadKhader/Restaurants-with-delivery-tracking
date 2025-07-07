using DotNetEnv;
using Payments.Data;
using Payments.Endpoints;
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
builder.Services.AddConventionalApplicationServices<Program, AppDbContext>();
builder.Host.ValidateScopes();

var app = builder.Build();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

app.MapPaymentsEndpoints();

app.Run();
public partial class Program { }