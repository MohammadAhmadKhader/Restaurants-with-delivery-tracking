using Auth.Models;

namespace Auth.Data.Seed.Converters;
public static class RestaurantPermissionConverter
{
    public static RestaurantPermission FromSeedPermission(SeedRestaurantPermission seedPermission)
    {
        var permission = new RestaurantPermission
        {
            NormalizedName = seedPermission.Name.ToUpperInvariant(),
            DisplayName = seedPermission.DisplayName,
            IsDefaultUser = seedPermission.IsDefaultUser,
            IsDefaultAdmin = seedPermission.IsDefaultAdmin,
            IsDefaultOwner = seedPermission.IsDefaultOwner,
        };

        return permission;
    }
}