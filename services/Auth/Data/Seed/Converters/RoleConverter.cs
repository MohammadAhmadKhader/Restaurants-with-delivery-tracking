using Auth.Models;

namespace Auth.Data.Seed.Converters;
public static class RoleConverter
{
    public static Role FromSeedRole(SeedRole seedRole)
    {
        var role = new Role
        {
            Name = seedRole.Name,
            DisplayName = seedRole.DisplayName,
            NormalizedName = seedRole.Name.ToUpperInvariant()
        };

        return role;
    }
}