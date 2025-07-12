using Shared.Contracts.Interfaces;

namespace Auth.Contracts.Dtos.Auth;

public class UserClaims: IUserClaims
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public HashSet<string> Roles { get; set; } = default!;
    public HashSet<string> Permissions { get; set; } = default!;
}