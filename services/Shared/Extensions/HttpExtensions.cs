using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Shared.Auth;
using Shared.Contracts.Interfaces;
using Restaurants.Contracts.Clients;
using Auth.Contracts.Clients;
using Refit;
using Shared.Config;

namespace Shared.Extensions;

public static class HttpExtensions
{
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

    public static IServiceCollection AddAuthClients(this IServiceCollection services)
    {
        var urls = MicroservicesUrlsProvider.Config;

        services.AddRefitClient<IAuthServiceClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(urls.AuthService);
            })
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddRefitClient<IUsersServiceClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(urls.AuthService);
            })
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddRefitClient<IRolesServiceClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(urls.AuthService);
            })
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddRefitClient<IAddressesServiceClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(urls.AuthService);
            })
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        services.AddScoped<IAuthProvider, AuthProvider>();

        return services;
    }

    public static IServiceCollection AddRestaurantsClients(this IServiceCollection services)
    {
        var urls = MicroservicesUrlsProvider.Config;

        services.AddRefitClient<IRestaurantServiceClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(urls.RestaurantsService);
            })
            .AddHttpMessageHandler<AuthenticationClientHandler>();

        return services;
    }

    public static IServiceCollection AddHttpClientsDependenciesWithClientsServices(this IServiceCollection services)
    {
        services.AddHttpClientsDependencies();

        services.AddAuthClients();
        services.AddRestaurantsClients();

        return services;
    }
}