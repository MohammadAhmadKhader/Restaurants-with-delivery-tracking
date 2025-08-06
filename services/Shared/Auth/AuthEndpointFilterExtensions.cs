using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Interfaces;

namespace Shared.Auth;

public static class AuthEndpointFilterExtensions
{
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string permission)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var userProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthProvider>();
            var userInfo = await userProvider.GetUserInfoAsync();

            if (userInfo.Permissions == null || !userInfo.Permissions.Contains(permission))
            {
                return Results.Problem(
                    title: "Forbidden",
                    detail: "You do not have permission to access this resource.",
                    statusCode: 403
                );
            }

            return await next(context);
        });
    }
    
    public static RouteHandlerBuilder RequireAuthenticatedUser(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var userProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthProvider>();
            var userInfo = await userProvider.GetUserInfoAsync();

            return await next(context);
        });
    }
}
