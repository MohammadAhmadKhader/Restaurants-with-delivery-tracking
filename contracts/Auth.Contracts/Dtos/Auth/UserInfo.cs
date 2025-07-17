using Shared.Contracts.Interfaces;

namespace Auth.Contracts.Dtos.Auth;

public class UserInfo : IUserInfo
{
    public Guid UserId { get; set; }
    public HashSet<string> Roles { get; set; } = default!;
    public HashSet<string> Permissions { get; set; } = default!;
    public Guid? RestaurantId { get; set; }
}