using Shared.Contracts.Attributes;
using Shared.Contracts.Interfaces;

namespace Auth.Contracts.Dtos.Auth;

public class UserClaims: IUserClaims
{
    public Guid UserId { get; set; }

    [Masked]
    public string Email { get; set; } = default!;
    public HashSet<string> Roles { get; set; } = default!;
}