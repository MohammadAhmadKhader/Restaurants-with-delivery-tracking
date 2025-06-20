using Auth.Contracts.Dtos.Role;
using Refit;
using Shared.Contracts.Dtos;

namespace Auth.Contracts.Clients;

public interface IRolesServiceClient
{
    [Get("/api/roles")]
    Task<CollectionResponse<RoleViewDto>> GetRoles([Query] PagedRequest pagedRequest);

    [Post("/api/roles")]
    Task<RoleResponseWrapper> CreateRole([Body] RoleCreateDto dto);

    [Put("/api/roles/{id}")]
    Task<ApiResponse<object>> UpdateRole(Guid id, [Body] RoleUpdateDto dto);

    [Delete("/api/roles/{id}")]
    Task<ApiResponse<object>> DeleteRole(Guid id, [Body] RoleUpdateDto dto);

    [Post("/api/roles/{id}/permissions")]
    Task<ApiResponse<object>> AddPermission(Guid id, [Body] RoleAddPermissionsDto dto);

    [Delete("/api/roles/{id}/permissions/{permissionId}")]
    Task<ApiResponse<object>> RemovePermission(Guid id, int permissionId);
}

public class RoleResponseWrapper
{
    public RoleViewDto Role { get; set; } = default!;
}