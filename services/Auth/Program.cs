using Auth.Models;
using Auth.Services;
using Auth.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Auth.Extensions.FluentValidation;
using Auth.Endpoints;
using Auth.Repositories.IRepositories;
using Auth.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using StackExchange.Redis;
using Auth.Data;
using Auth.Extensions;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using Shared.Middlewares;
using Auth.Dtos;
using Auth.Utils;
using Role = Auth.Models.Role;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>((options) =>
{
    var config = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(config))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured");
    }
    options.UseNpgsql(config);
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IAddressesRepository, AddressesRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    if (jwtSettings == null)
    {
        throw new InvalidOperationException("JwtSettings are not configured");
    }

    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = SecurityUtils.CreateValidationTokenParams(jwtSettings);
});

builder.Services.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher<User>>();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
    .WriteTo.Console(
        theme: AnsiConsoleTheme.Sixteen,
        applyThemeToRedirectedOutput: true
    )
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<Role>()
.AddRoleManager<RoleManager<Role>>()
.AddSignInManager<SignInManager<User>>()
.AddUserManager<UserManager<User>>()
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var url = builder.Configuration.GetConnectionString("Redis");
    if (string.IsNullOrWhiteSpace(url))
    {
        throw new InvalidOperationException("Connection string 'Redis' is not configured");
    }

    return ConnectionMultiplexer.Connect(url);
});

builder.Services.AddFluentValidation();
builder.Services.AddSeeding();
var app = builder.Build();

// auth
app.UseAuthentication();
app.UseAuthorization();

// end points
app.MapAuthEndpoints();
app.MapUsersEndpoints();

// middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();

// others
await app.SeedDatabaseAsync();

app.Run();