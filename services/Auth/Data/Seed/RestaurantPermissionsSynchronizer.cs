using Auth.Data.Seed.Converters;
using Auth.Models;
using Auth.Utils;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Extensions;
using Shared.Utils;

namespace Auth.Data.Seed;

public class RestaurantPermissionsSynchronizer(
    AppDbContext ctx,
    Serilog.ILogger logger,
    IUnitOfWork<AppDbContext> uow
) : IRestaurantPermissionsSynchronizer
{
    private readonly AppDbContext _ctx = ctx;
    private readonly IUnitOfWork<AppDbContext> _uow = uow;
    private readonly Serilog.ILogger _logger = logger;
    public async Task SyncPermissionsAsync()
    {
        _logger.Information("Starting to sync restaurants permissions...");
        var jsonPermissions = (await DataLoader.GetSeedData()).RestaurantPermissions;
        var appPermissions = GeneralUtils.GetAllStringConstants(typeof(RestaurantPermissions));

        // checking in case there is a permission is defined in RestaurantPermissions and has no configuration inside the json file.
        var jsonPermissionsNamesHashSet = jsonPermissions.Select(x => x.Name.ToUpper()).ToHashSet();
        foreach (var appPermission in appPermissions)
        {  
            if (!jsonPermissionsNamesHashSet.Contains(appPermission))
            {
                _logger.Error("Missing permission in json '{Name}' you must define its configuration inside the json file.", appPermission);
                Environment.Exit(1);
            }
        }

        var dbPermissions = await _ctx.RestaurantPermissions
                                    .ToListAsync();
        var dbRoles = await _ctx.RestaurantRoles
                            .IgnoreQueryFilters()
                            .Include(r => r.Permissions)
                            .ToListAsync();

        using var tx = await _uow.BeginTransactionAsync();

        // sync-ing and reseting if changed permission settings
        var dbPermissionsNamesHashSet = dbPermissions.Select(x => x.NormalizedName);
        foreach (var role in dbRoles)
        {
            // updating existing permissions
            foreach (var dbPermission in role.Permissions)
            {
                var jsonPermission = jsonPermissions.Where(p => p.Name.ToUpper() == dbPermission.NormalizedName).FirstOrDefault();
                if (jsonPermission == null)
                {
                    _logger.Warning("Json permission with name '{Name}' was not found.", dbPermission.NormalizedName);
                    continue;
                }

                dbPermission.IsDefaultUser = jsonPermission.IsDefaultUser;
                dbPermission.IsDefaultAdmin = jsonPermission.IsDefaultAdmin;
                dbPermission.IsDefaultOwner = jsonPermission.IsDefaultOwner;
                dbPermission.DisplayName = jsonPermission.DisplayName;
                dbPermission.NormalizedName = jsonPermission.Name.ToUpperInvariant();
            }

            // adding new permissions
            foreach (var jsonPermission in jsonPermissions)
            {
                if (!dbPermissionsNamesHashSet.Contains(jsonPermission.Name.ToUpper()))
                {
                    var permission = RestaurantPermissionConverter.FromSeedPermission(jsonPermission);
                    var addedPerm = _ctx.RestaurantPermissions.Add(permission);
                    dbPermissions.Add(addedPerm.Entity);
                }
            }
        }

        await GeneralUtils.LogOnErrorAsync(
            async () => await _uow.SaveChangesAsync(),
            "Failed during [Syncing & Reseting changed permissions] stage");

        // removing old relations
        foreach (var dbRole in dbRoles)
        {
            dbRole.Permissions.Clear();
        }

        await GeneralUtils.LogOnErrorAsync(
            async () => await _uow.SaveChangesAsync(),
            "Failed during [Removing Old Relations] stage");

        // removing obselete/re-named permissions from db.
        var deletedPermissions = new List<RestaurantPermission>();
        foreach (var dbPermission in dbPermissions)
        {
            if (!appPermissions.Contains(dbPermission.NormalizedName))
            {
                _ctx.Remove(dbPermission);
                deletedPermissions.Add(dbPermission);
            }
        }

        // removing the deleted perms from the list.
        dbPermissions.RemoveAll(p => deletedPermissions.Contains(p));

        await GeneralUtils.LogOnErrorAsync(
            async () => await _uow.SaveChangesAsync(),
            "Failed during [Removing Obselete/Renamed permissions] stage"
        );

        // re-adding roles
        foreach (var dbRole in dbRoles)
        {
            if (SecurityUtils.IsRestCustomerRole(dbRole))
            {
                var rolePermissions = dbPermissions.Where(x => x.IsDefaultUser).ToList();
                dbRole.Permissions.AddRange(rolePermissions);
            }

            if (SecurityUtils.IsRestAdminRole(dbRole))
            {
                var rolePermissions = dbPermissions.Where(x => x.IsDefaultAdmin).ToList();
                dbRole.Permissions.AddRange(rolePermissions);
            }

            if (SecurityUtils.IsRestOwnerRole(dbRole))
            {
                var rolePermissions = dbPermissions.Where(x => x.IsDefaultOwner).ToList();
                dbRole.Permissions.AddRange(rolePermissions);
            }
        }

        await GeneralUtils.LogOnErrorAsync(
            async () => await _uow.SaveChangesAsync(),
            "Failed during [Re-Adding] stage");

        await _uow.CommitTransactionAsync(tx);

        _logger.Information("Roles were synced successfully, ");

        Environment.Exit(0);
    }
}