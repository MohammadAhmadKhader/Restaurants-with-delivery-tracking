using Auth.Contracts.Dtos.RestaurantRole;

namespace Auth.Contracts.Dtos.User;

public class UserWithRestaurantRolesAndPermissionsDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IList<RestaurantRoleWithPermissionViewDto> Roles { get; set; } = [];
}