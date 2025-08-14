using Auth.Contracts.Dtos.RestaurantRole;
using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.User;

public class UserWithRestaurantRolesAndPermissionsDto
{
    public Guid Id { get; set; }

    [Masked]
    public string? Email { get; set; }

    [Masked]
    public string? FirstName { get; set; }

    [Masked]
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IList<RestaurantRoleWithPermissionViewDto> Roles { get; set; } = [];
}