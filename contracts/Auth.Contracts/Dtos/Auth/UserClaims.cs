namespace Auth.Contracts.Dtos.Auth;

public class UserClaims
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public IEnumerable<string> Roles { get; set; } = default!;
    public IEnumerable<string> Permissions { get; set; } = default!;
}