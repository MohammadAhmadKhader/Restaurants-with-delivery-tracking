using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);
var host = builder.Host;

builder.Configuration.AddJsonFile("./Properties/ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddServiceLogging(host);

var app = builder.Build();

// app.UseHttpsRedirection();
await app.UseOcelot();

app.Run();