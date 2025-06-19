using Auth.Contracts.Dtos.Permission;

namespace Auth.Contracts.Dtos.Role;

public class RoleWithPermissionViewDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public IList<PermissionViewDto> Permissions { get; set; } = [];
}