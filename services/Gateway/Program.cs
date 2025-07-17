using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared.Extensions;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);
var host = builder.Host;
var config = builder.Configuration;

builder.Configuration.AddGlobalConfig();
builder.Configuration.AddJsonFile("./Properties/ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddServiceLogging(host, config);
builder.Host.ValidateScopes();
builder.Services.AddCors();

var app = builder.Build();
app.UseSerilogRequestLoggingWithTraceId();
app.UseCors(options =>
{
    options.WithOrigins("http://localhost", "http://127.0.0.1");
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowCredentials();
});

await app.UseOcelot();

// app.UseHttpsRedirection();

app.Run();