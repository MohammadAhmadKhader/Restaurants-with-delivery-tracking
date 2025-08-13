using Notifications.Services;
using Notifications.Services.IServices;

namespace Notifications.Extensions;
public static class ServicesExtensions
{
    public static IServiceCollection AddEmailTemplatesService(this IServiceCollection services)
    {
        services.AddSingleton<IEmailTemplatesService, EmailTemplatesService>();
        return services;
    }
}