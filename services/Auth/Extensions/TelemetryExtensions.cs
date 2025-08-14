using Auth.Services.IServices;
using Shared.Observability.Telemetry;

namespace Auth.Extensions;
public static class TelemetryExtensions
{
    public static IServiceCollection AddServicesWithTelemetry(this IServiceCollection services)
    {
        List<Type> servicesTypes = [
            typeof(IAuthService), typeof(IAddressesService), typeof(IRolesService),
            typeof(ITokenService), typeof(IUsersService), typeof(IRestaurantRolesService),
            typeof(IPasswordResetTokensService)
        ];

        services.DecorateWithTracing(servicesTypes);

        return services;
    }
}