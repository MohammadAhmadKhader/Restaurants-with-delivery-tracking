using Payments.Services;
using Payments.Services.IServices;

namespace Payments.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentsService, PaymentsService>();

        return services;
    }
}