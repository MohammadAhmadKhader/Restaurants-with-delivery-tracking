namespace Shared.Contracts.Interfaces;

public interface IUserClaims
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public HashSet<string> Roles { get; set; }
    public HashSet<string> Permissions { get; set; }
}