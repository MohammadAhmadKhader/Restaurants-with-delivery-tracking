using Payments.Data;
using Payments.Repositories;
using Payments.Repositories.IRepositories;
using Shared.Data.Patterns.UnitOfWork;

namespace Payments.Extensions;
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        services.AddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        
        return services;
    }
}