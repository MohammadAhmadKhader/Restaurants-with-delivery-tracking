using Locations.Repositories;
using Locations.Repositories.IRepositories;

namespace Locations.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICurrentLocationsRepository, CurrentLocationsRepository>();
        services.AddScoped<ILocationsHistoriesRepostiory, LocationsHistoriesRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}