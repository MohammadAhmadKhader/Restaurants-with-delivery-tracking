using Auth.Models;

namespace Auth.Data.Seed;

public interface IDatabaseSeeder
{
    Task SeedAsync();
    Task SeedTenantRolesAsync(
        Guid restaurantId,
        Func<(
        RestaurantRole customerRole,
        RestaurantRole adminRole,
        RestaurantRole ownerRole), Task>? actionBeforeCommit = null);
}
