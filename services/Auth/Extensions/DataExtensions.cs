using Auth.Data.Seed;
using Shared.Utils;

namespace Auth.Extensions;

public static class DataExtensions
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

    public static async Task UseRestaurantPermissionsSynchronizer(this IServiceCollection services)
    {
        var args = Environment.GetCommandLineArgs();
        var hasSyncCommand = args.Contains("--sync-permissions") || args.Contains("-sp");
        if (!hasSyncCommand || !EnvironmentUtils.IsDevelopment())
        {
            return;
        }

        services.AddSingleton<IRestaurantPermissionsSynchronizer, RestaurantPermissionsSynchronizer>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantPermissionsSynchronizer>();

        await GeneralUtils.ActionOnThrowAsync(seeder.SyncPermissionsAsync, () => Environment.Exit(1));
    }
}