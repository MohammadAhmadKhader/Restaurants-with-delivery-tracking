using Auth.Dtos.Role;

namespace Auth.Dtos.User;

public class UserWithRolesAndPermissionsDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IList<RoleWithPermissionViewDto> Roles { get; set; } = [];
}