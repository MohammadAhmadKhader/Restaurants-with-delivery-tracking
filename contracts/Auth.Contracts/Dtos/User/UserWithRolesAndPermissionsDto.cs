using Auth.Contracts.Dtos.Role;
using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.User;

public class UserWithRolesAndPermissionsDto
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
    public IList<RoleWithPermissionViewDto> Roles { get; set; } = [];
}