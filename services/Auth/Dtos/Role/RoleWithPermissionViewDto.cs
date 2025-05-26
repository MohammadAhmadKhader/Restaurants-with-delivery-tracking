using Auth.Dtos.Permission;

namespace Auth.Dtos.Role;

public class RoleWithPermissionViewDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public IList<PermissionViewDto> Permissions { get; set; } = [];
}