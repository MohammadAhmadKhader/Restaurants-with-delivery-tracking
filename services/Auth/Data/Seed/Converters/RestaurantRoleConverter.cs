using Auth.Models;

namespace Auth.Data.Seed.Converters;

public class RestaurantRoleConverter
{
    public static RestaurantRole FromSeedRole(SeedRestaurantRole seedRole, Guid restaurantId)
    {
        var role = new RestaurantRole()
        {
            DisplayName = seedRole.DisplayName,
            NormalizedName = seedRole.Name.ToUpperInvariant(),
            RestaurantId = restaurantId,
        };

        return role;
    }
}