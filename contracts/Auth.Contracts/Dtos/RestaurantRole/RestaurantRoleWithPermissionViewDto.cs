using Auth.Contracts.Dtos.Permission;
using Auth.Contracts.Dtos.RestaurantPermission;

namespace Auth.Contracts.Dtos.RestaurantRole;

public class RestaurantRoleWithPermissionViewDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public IList<RestaurantPermissionViewDto> Permissions { get; set; } = [];
}