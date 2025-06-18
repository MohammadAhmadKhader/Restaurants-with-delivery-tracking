using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Shared.Http;
using Shared.Http.Clients;
using Shared.Interfaces;

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

    public static IServiceCollection AddHttpClientWithClientServices(this IServiceCollection services)
    {
        services.AddTransient<AuthenticationClientHandler>();
        services.AddHttpClient<IHttpClientService, HttpClientService>()
                .AddHttpMessageHandler<AuthenticationClientHandler>();
        services.AddScoped<ITokenProvider, TokenProvider>();

        services.AddSingleton<IRestaurantServiceClient, RestaurantServiceClient>();
        services.AddSingleton<IAuthServiceClient, AuthServiceClient>();

        return services;
    }
}