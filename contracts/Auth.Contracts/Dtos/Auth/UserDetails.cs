using Shared.Contracts.Attributes;
using Shared.Contracts.Interfaces;

namespace Auth.Contracts.Dtos.Auth;

public class UserDetails : IUserDetails
{
    public Guid UserId { get; set; }
    
    [Masked]
    public string Email { get; set; } = default!;

    [Masked]
    public string FirstName { get; set; } = default!;

    [Masked]
    public string LastName { get; set; } = default!;
}