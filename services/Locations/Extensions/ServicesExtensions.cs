using Locations.Services;
using Locations.Services.IServices;

namespace Locations.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ILocationsService, LocationsService>();

        return services;
    }
}