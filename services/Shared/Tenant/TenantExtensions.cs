using Microsoft.Extensions.DependencyInjection;

namespace Shared.Tenant;
public static class TenantExtensions
{
    public static IServiceCollection AddTenantProvider(this IServiceCollection services)
    {
        services.AddScoped<ITenantProvider, TenantProvider>();
        return services;
    }
}