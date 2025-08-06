using Auth.Services.IServices;
using Auth.Mappers;
using Auth.Contracts.Dtos.User;
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
            PaginationUtils.NormalizeAndModify(filterParams);
            sortUils.Normalize(filterParams);

            var (users, count) = await usersService.FilterUsersAsync(filterParams);
            var usersViews = users.Select(u => u.ToViewWithRolesDto()).ToList();

            return Results.Ok(PaginationUtils.ResultOf(usersViews, count, filterParams.Page, filterParams.Size));
        }).RequireAuthorization(RolePolicies.AdminsRolesPolicy)
        .AddEndpointFilter<ValidationFilter<UsersFilterParams>>();


        group.MapPatch("/delete/{id}", async (Guid id, IUsersService usersService) =>
        {
            var (isSuccess, error) = await usersService.DeleteByIdAsync(id);
            if (!isSuccess)
            {
                var code = error.GetStatusCode();
                var message = error.GetMessage();
                return ResponseUtils.Error(message, code);
            }
            
            return Results.NoContent(); 
        }).RequireAuthorization(RolePolicies.AdminsRolesPolicy);

        group.MapPatch("/self-delete", async (ClaimsPrincipal principal, IUsersService usersService) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            if (userId == Guid.Empty)
            {
                return ResponseUtils.Forbidden();
            }

            var (isSuccess, error) = await usersService.DeleteByIdAsync(userId);
            if (!isSuccess)
            {
                var code = error.GetStatusCode();
                var message = error.GetMessage();
                return ResponseUtils.Error(message, code);
            }
            
            return Results.NoContent(); 
        }).RequireAuthorization();

        group.MapGet("/{id}", async (Guid id, IUsersService usersService) =>
        {
            var user = await usersService.FindByIdWithRolesAndPermissionsAsync(id);
            if (user == null)
            {
                return ResponseUtils.NotFound("user");
            }
  
            return Results.Ok(new { user = user.ToViewWithRolesAndPermissionsDto() });
        }).RequireAuthorization(RolePolicies.AdminsRolesPolicy);

        group.MapPut("/profile", async (IUsersService usersService, UserUpdateProfile dto, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            if (userId == Guid.Empty)
            {
                return ResponseUtils.Forbidden();
            }

            await usersService.UpdateProfileAsync(userId, dto);

            return Results.NoContent();
        }).RequireAuthorization()
        .AddEndpointFilter<ValidationFilter<UserUpdateProfile>>();
    }
}