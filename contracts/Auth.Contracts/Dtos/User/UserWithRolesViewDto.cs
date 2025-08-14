using Auth.Contracts.Dtos.Role;
using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.User;

public class UserWithRolesDto
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
    
    public IList<RoleViewDto> Roles { get; set; } = [];
}