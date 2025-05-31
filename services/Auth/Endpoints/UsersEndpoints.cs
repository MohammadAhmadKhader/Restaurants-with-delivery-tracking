using Auth.Services.IServices;
using Auth.Mappers;
using Auth.Dtos.User;
using Shared.Filters;
using Shared.Utils;
using Auth.Utils;
using Auth.Extensions;
using System.Security.Claims;

namespace Auth.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");
        var sortUils = new SortingUtils(["firstName", "lastName", "createdAt"]);

        group.MapGet("/", async (IUsersService usersService, [AsParameters] UsersFilterParams filterParams) =>
        {
            PaginationUtils.Normalize(filterParams);
            sortUils.Normalize(filterParams);

            var (users, count) = await usersService.FilterUsersAsync(filterParams);
            var usersViews = users.Select(u => u.ToViewWithRolesDto()).ToList();

            return Results.Ok(PaginationUtils.ResultOf(usersViews, count, filterParams.Page, filterParams.Size));
        }).RequireAuthorization(policy => policy.RequireRole(RolePolicies.Admin, RolePolicies.SuperAdmin))
        .AddEndpointFilter<ValidationFilter<UsersFilterParams>>();


        group.MapPatch("/delete/{id:guid}", async (Guid id, IUsersService usersService) =>
        {
            var (isSuccess, error) = await usersService.DeleteById(id);
            if (!isSuccess)
            {
                var code    = error.GetStatusCode();
                var message = error.GetMessage();
                return Results.Json(new { detail = message }, statusCode: code);
            }
            
            return Results.NoContent(); 
        }).RequireAuthorization(policy => policy.RequireRole(RolePolicies.AdminRoles));

        group.MapPatch("/self-delete", async (ClaimsPrincipal principal, IUsersService usersService) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            if (userId == Guid.Empty)
            {
                return Results.Forbid();
            }

            var (isSuccess, error) = await usersService.DeleteById(userId);
            if (!isSuccess)
            {
                var code    = error.GetStatusCode();
                var message = error.GetMessage();
                return Results.Json(new { detail = message}, statusCode: code);
            }
            
            return Results.NoContent(); 
        }).RequireAuthorization();


        group.MapGet("/{id:guid}", async (Guid id, IUsersService usersService) =>
        {
            var user = await usersService.FindByIdWithRolesAndPermissions(id);
            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new { user = user.ToViewWithRolesAndPermissionsDto() });
        }).RequireAuthorization(policy => policy.RequireRole(RolePolicies.AdminRoles));


        group.MapPut("/profile", async (IUsersService usersService, UserUpdateProfile dto, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            if (userId == Guid.Empty)
            {
                return Results.Forbid();
            }

            var updatedUser = await usersService.UpdateProfile(userId, dto);
            if(updatedUser == null)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }).RequireAuthorization();
    }
}