using Auth.Services;
using Auth.Services.IServices;

namespace Auth.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRolesService, RolesService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IAddressesService, AddressesService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}