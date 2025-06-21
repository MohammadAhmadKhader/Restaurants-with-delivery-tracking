using Orders.Data;
using Orders.Repositories;
using Orders.Repositories.IRepositories;
using Shared.Data.Patterns.UnitOfWork;

namespace Orders.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        
        return services;
    }
}