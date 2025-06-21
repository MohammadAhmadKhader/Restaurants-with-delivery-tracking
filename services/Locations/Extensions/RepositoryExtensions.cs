using Locations.Data;
using Locations.Repositories;
using Locations.Repositories.IRepositories;
using Shared.Data.Patterns.UnitOfWork;

namespace Locations.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICurrentLocationsRepository, CurrentLocationsRepository>();
        services.AddScoped<ILocationsHistoriesRepostiory, LocationsHistoriesRepository>();
        services.AddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        
        return services;
    }
}