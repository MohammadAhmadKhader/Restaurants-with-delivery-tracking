using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("./Properties/ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Configuration.AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Sixteen,
        applyThemeToRedirectedOutput: true
    )
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// app.UseHttpsRedirection();
await app.UseOcelot();

app.Run();