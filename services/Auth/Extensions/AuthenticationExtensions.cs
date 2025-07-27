using Auth.Config;
using Auth.Data;
using Auth.Models;
using Auth.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Shared.Utils;

namespace Auth.Extensions;
public static class AuthenticationExtensions
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettingsSection = config.GetSection("JwtSettings");

        services.Configure<JwtSettings>(jwtSettingsSection);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            GuardUtils.ThrowIfNull(jwtSettings);

            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = SecurityUtils.CreateValidationTokenParams(jwtSettings);

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    var errorDetail = string.IsNullOrEmpty(context.Request.Headers.Authorization)
                            ? "Authorization header is missing"
                            : "Invalid or expired token";

                    var problem = Results.Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "Unauthorized",
                        detail: errorDetail
                    );

                    await problem.ExecuteAsync(context.HttpContext);
                },
                OnMessageReceived = context =>
                {
                    var endpoint = context.HttpContext.GetEndpoint();
                    var requiresAuth = endpoint?.Metadata?.GetMetadata<IAuthorizeData>() != null;
                    if (!requiresAuth)
                    {
                        context.NoResult();
                    }

                    return Task.CompletedTask;
                },
            };
        });

        services.AddIdentityCore<User>(options =>
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
        
        services.AddHttpContextAccessor();
        services.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher<User>>();

        return services;
    }
}