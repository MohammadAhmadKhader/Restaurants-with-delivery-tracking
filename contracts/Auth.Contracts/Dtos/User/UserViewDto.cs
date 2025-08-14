using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.User;

public class UserViewDto
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
}