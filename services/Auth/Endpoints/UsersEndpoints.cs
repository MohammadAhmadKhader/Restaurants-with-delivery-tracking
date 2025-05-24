using Auth.Services.IServices;

namespace Auth.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/", async (IUsersService usersService) =>
        {
        
            return Results.Ok();
        });
    }
}