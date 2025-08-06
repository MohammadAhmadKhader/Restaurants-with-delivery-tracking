using Auth.Contracts.Enums;
using Shared.Contracts.Interfaces;

namespace Auth.Contracts.Dtos.Auth;

public class UserInfo : IUserInfo
{
    public static readonly string RestaurantAdmin = DefaultRestaurantUserRoles.Admin.ToString().ToUpperInvariant();
    public static readonly string RestaurantOwner = DefaultRestaurantUserRoles.Owner.ToString().ToUpperInvariant();
    public static readonly string Admin = DefaultUserRoles.Admin.ToString().ToUpperInvariant();
    public static readonly string SuperAdmin = DefaultUserRoles.SuperAdmin.ToString().ToUpperInvariant();

    public Guid UserId { get; set; }
    public HashSet<string> Roles { get; set; } = default!;
    public HashSet<string> Permissions { get; set; } = default!;
    public Guid? RestaurantId { get; set; }

    public bool IsSuperAdmin() => Roles.Contains(SuperAdmin);
    public bool IsAdmin() => Roles.Contains(Admin) && RestaurantId == null;
    public bool HasAdminsRoles() => IsAdmin() || IsSuperAdmin();

    public bool IsRestaurantOwner() => Roles.Contains(RestaurantOwner);
    public bool IsRestaurantAdmin() => Roles.Contains(RestaurantAdmin) && RestaurantId != null;
    public bool HasRestaurantAdminsRoles() => IsRestaurantAdmin() || IsRestaurantOwner();  
}