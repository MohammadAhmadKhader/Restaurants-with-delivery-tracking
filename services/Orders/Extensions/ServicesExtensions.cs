using Orders.Services;
using Orders.Services.IServices;

namespace Orders.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersService>();

        return services;
    }
}