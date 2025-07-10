using Auth.Contracts.Dtos.Role;
using Auth.Services.IServices;
using Shared.Contracts.Dtos;
using Shared.Utils;
using Auth.Mappers;
using Auth.Utils;
using Shared.Filters;

namespace Auth.Endpoints;

public static class RolesEndpoints
{
    public static void MapRolesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles").RequireAuthorization(RolePolicies.SuperAdminPolicy);

        group.MapGet("", async ([AsParameters] PagedRequest pagedReq, IRolesService rolesService) =>
        {
            PaginationUtils.Normalize(pagedReq);
            var page = pagedReq.Page!.Value;
            var size = pagedReq.Size!.Value;
            
            var (roles, count) = await rolesService.FindAllAsync(page, size);
            var rolesViews = roles.Select(r => r.ToViewDto()).ToList();

            return Results.Ok(PaginationUtils.ResultOf(rolesViews, count, page, size));
        });

        group.MapPost("", async (RoleCreateDto dto, IRolesService rolesService) =>
        {
            var role = await rolesService.CreateAsync(dto);

            return Results.Ok(new { role = role.ToViewDto() });
        }).AddEndpointFilter<ValidationFilter<RoleCreateDto>>();

        group.MapPut("/{id}", async (Guid id, RoleUpdateDto dto, IRolesService rolesService) =>
        {
            await rolesService.UpdateAsync(id, dto);

            return Results.NoContent();
        }).AddEndpointFilter<ValidationFilter<RoleUpdateDto>>();

        group.MapDelete("/{id}", async (Guid id, IRolesService rolesService) =>
        {
            await rolesService.DeleteAsync(id);

            return Results.NoContent();
        });

        group.MapPost("/{id}/permissions", async (Guid id, RoleAddPermissionsDto dto, IRolesService rolesService) =>
        {
            await rolesService.AddPermissions(id, dto.Ids);

            return Results.NoContent();
        }).AddEndpointFilter<ValidationFilter<RoleAddPermissionsDto>>();

        group.MapDelete("/{id}/permissions/{permissionId}", async (Guid id, int permissionId, IRolesService rolesService) =>
        {
            await rolesService.RemovePermission(id, permissionId);
            
            return Results.NoContent();
        });
    }
}