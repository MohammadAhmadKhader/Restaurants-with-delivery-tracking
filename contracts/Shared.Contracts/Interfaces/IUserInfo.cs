namespace Shared.Contracts.Interfaces;

public interface IUserInfo
{
    public Guid UserId { get; set; }
    public HashSet<string> Roles { get; set; }
    public HashSet<string> Permissions { get; set; }
    public Guid? RestaurantId { get; set; }
}