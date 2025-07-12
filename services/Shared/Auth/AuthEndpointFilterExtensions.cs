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
            var claims = await userProvider.GetUserClaimsAsync();

            if (claims.Permissions == null || !claims.Permissions.Contains(permission))
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
}
