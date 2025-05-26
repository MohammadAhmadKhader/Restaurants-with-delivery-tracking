namespace Auth.Dtos.Auth;

public class UserClaims
{
    public string Email { get; set; } = default!;
    public IEnumerable<string> Roles { get; set; } = default!;
    public IEnumerable<string> Permissions { get; set; } = default!;
}