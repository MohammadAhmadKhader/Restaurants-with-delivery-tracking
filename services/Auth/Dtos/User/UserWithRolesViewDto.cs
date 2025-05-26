using Auth.Dtos.Role;

namespace Auth.Dtos.User;

public class UserWithRolesDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IList<RoleViewDto> Roles { get; set; } = [];
}