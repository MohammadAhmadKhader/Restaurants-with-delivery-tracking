using Payments.Repositories;
using Payments.Repositories.IRepositories;

namespace Payments.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}