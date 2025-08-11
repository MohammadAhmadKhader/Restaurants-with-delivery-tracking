using Shared.Contracts.Interfaces;

namespace Auth.Contracts.Dtos.Auth;

public class UserDetails : IUserDetails
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}