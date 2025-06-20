using Auth.Endpoints;
using Auth.Data;
using Auth.Extensions;
using Shared.Middlewares;
using Shared.Extensions;
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var host = builder.Host;

builder.Services.AddControllers();
builder.Services.AddNamingPolicy();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddAppAuthentication(config);
builder.Services.AddKvStore(config, "Redis");
builder.Services.AddDatabase<AppDbContext>(config, "DefaultConnection");
builder.Services.AddServiceLogging(host);
builder.Services.AddFluentValidation();
builder.Services.AddSeeding();
builder.Services.AddAppProblemDetails();
builder.Services.AddHttpClientsDependenciesWithClientsServices();

var app = builder.Build();

// auth
app.UseAuthentication();
app.UseAuthorization();

// end points
app.MapAuthEndpoints();
app.MapUsersEndpoints();
app.MapRolesEndpoints();
app.MapAddressesEndpoints();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAppLocalization();

// others
await app.SeedDatabaseAsync();

app.Run();
public partial class Program { }