using Auth.Services.IServices;
using Auth.Mappers;
using Auth.Dtos.User;
using Shared.Filters;
using Shared.Utils;

namespace Auth.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/", async (IUsersService usersService, [AsParameters] UsersFilterParams filterParams) =>
        {
            PaginationUtils.Normalize(filterParams);
            
            var (users, count) = await usersService.FilterUsersAsync(filterParams);
            var usersViews = users.Select(u => u.ToViewWithRolesDto()).ToList();
            
            return Results.Ok(PaginationUtils.ResultOf(usersViews, count, filterParams.Page, filterParams.Size));
        }).RequireAuthorization()
        .AddEndpointFilter<ValidationFilter<UsersFilterParams>>();
    }
}