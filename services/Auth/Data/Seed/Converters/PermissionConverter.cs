using Auth.Models;

namespace Auth.Data.Seed.Converters;
public static class PermissionConverter
{
    public static Permission FromSeedPermission(SeedPermission seedPermission)
    {
        var permission = new Permission
        {
            Name = seedPermission.Name.ToUpperInvariant(),
            DisplayName = seedPermission.DisplayName,
            IsDefaultUser = seedPermission.IsDefaultUser,
            IsDefaultAdmin = seedPermission.IsDefaultAdmin,
            IsDefaultSuperAdmin = seedPermission.IsDefaultSuperAdmin,
        };

        return permission;
    }
}