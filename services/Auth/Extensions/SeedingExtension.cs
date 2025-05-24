using Auth.Data.Seed;

namespace Auth.Extensions;

public static class SeedingExtension
{
    public static IServiceCollection AddSeeding(this IServiceCollection services)
    {
        services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
        return services;
    }

    public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();
    }
}