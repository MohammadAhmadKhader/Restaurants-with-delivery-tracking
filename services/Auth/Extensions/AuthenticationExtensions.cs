using Auth.Config;
using Auth.Data;
using Auth.Models;
using Auth.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace Auth.Extensions;
public static class AuthenticationExtensions
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JwtSettings are not configured");
            }

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
                }
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