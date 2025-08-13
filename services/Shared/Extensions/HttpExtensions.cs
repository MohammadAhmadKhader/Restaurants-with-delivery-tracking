using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Shared.Auth;
using Shared.Contracts.Interfaces;
using Restaurants.Contracts.Clients;
using Auth.Contracts.Clients;
using Refit;
using Shared.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Payments.Contracts.Clients;
using Orders.Contracts.Clients;
using Notifications.Contracts.Clients;

namespace Shared.Extensions;

public static class HttpExtensions
{
    private static Action<HttpClient> GetDefaultRefitOptions(string url)
    {
        return client =>
        {
            client.BaseAddress = new Uri(url);
        };
    }

    public static IServiceCollection AddAppProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
                ctx.ProblemDetails.Extensions.TryAdd("requestId", ctx.HttpContext.TraceIdentifier);

                var activity = ctx.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                ctx.ProblemDetails.Extensions.TryAdd("traceId", activity?.TraceId);
            };
        });

        return services;
    }

    public static IServiceCollection AddHttpClientsDependencies(this IServiceCollection services)
    {
        services.AddTransient<AuthenticationClientHandler>();
        services.AddHttpContextAccessor();
        services.AddScoped<ITokenProvider, TokenProvider>();

        return services;
    }

    public static IServiceCollection AddAuthClients(this IServiceCollection services, IConfiguration config)
    {
        var urls = MicroservicesUrlsProvider.GetUrls(config);
        services.AddRefitClient<IAuthServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.AuthService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddRefitClient<IUsersServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.AuthService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddRefitClient<IRolesServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.AuthService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddRefitClient<IAddressesServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.AuthService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddAuthProvider();

        return services;
    }

    public static IServiceCollection AddRestaurantsClients(this IServiceCollection services, IConfiguration config)
    {
        var urls = MicroservicesUrlsProvider.GetUrls(config);

        services.AddRefitClient<IRestaurantServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.RestaurantsService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddRefitClient<IMenusServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.RestaurantsService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        return services;
    }

    public static IServiceCollection AddPaymentsClients(this IServiceCollection services, IConfiguration config)
    {
        var urls = MicroservicesUrlsProvider.GetUrls(config);

        services.AddRefitClient<IPaymentsServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.PaymentsService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        return services;
    }

    public static IServiceCollection AddOrdersClients(this IServiceCollection services, IConfiguration config)
    {
        var urls = MicroservicesUrlsProvider.GetUrls(config);

        services.AddRefitClient<IOrdersServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.OrdersService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        return services;
    }

    public static IServiceCollection AddNotificationsClients(this IServiceCollection services, IConfiguration config)
    {
        var urls = MicroservicesUrlsProvider.GetUrls(config);

        services.AddRefitClient<INotificationsServiceClient>()
            .ConfigureHttpClient(GetDefaultRefitOptions(urls.NotificationsService))
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        return services;
    }

    public static IServiceCollection AddServicesClientsWithDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClientsDependencies();

        services.AddAuthClients(config);
        services.AddRestaurantsClients(config);
        services.AddPaymentsClients(config);
        services.AddOrdersClients(config);
        services.AddNotificationsClients(config);

        return services;
    }

    public static IServiceCollection AddAuthProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IAuthProvider, AuthProvider>();
        return services;
    }
}