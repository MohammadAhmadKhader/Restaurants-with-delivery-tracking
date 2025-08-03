using System.Text.Json;
using Auth.Data.Seed.Converters;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Utils;
using Microsoft.AspNetCore.Identity;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Extensions;
using Shared.Utils;

namespace Auth.Data.Seed;

public class DatabaseSeeder(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IUnitOfWork<AppDbContext> unitOfWork,
    ILogger<DatabaseSeeder> logger,
    IRolesRepository rolesRepository,
    IPermissionsRepository permissionsRepository,
    IRestaurantRolesRepository restaurantRolesRepository,
    IRestaurantPermissionsRepository restaurantPermissionsRepository,
    ITenantProvider tenantProvider) : IDatabaseSeeder
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly ILogger<DatabaseSeeder> _logger = logger;
    private readonly IRolesRepository _rolesRepository = rolesRepository;
    private readonly IPermissionsRepository _permissionsRepository = permissionsRepository;
    private readonly IRestaurantRolesRepository _restaurantRolesRepository = restaurantRolesRepository;
    private readonly IRestaurantPermissionsRepository _restaurantPermissionsRepository = restaurantPermissionsRepository;
    private readonly ITenantProvider _tenantProvider = tenantProvider;

    /// <summary>
    /// This method will seed users, roles and permissions then shutdown the application. 
    /// </summary>
    public async Task SeedAsync()
    {
        if (!EnvironmentUtils.IsSeeding())
        {
            _logger.LogInformation("Skipping seeding");
            return;
        }

        _logger.LogInformation("Starting to seed data...");

        var seedData = await DataLoader.GetSeedData();

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var role in seedData.Roles)
            {
                var rolesExists = await _roleManager.RoleExistsAsync(role.Name);
                if (rolesExists)
                {
                    continue;
                }

                var roleResult = await _roleManager.CreateAsync(RoleConverter.FromSeedRole(role));

                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role: {role.Name}");
                }
            }

            foreach (var userSeed in seedData.Users)
            {
                var userExists = await _userManager.FindByEmailAsync(userSeed.Email) != null;
                if (userExists)
                {
                    continue;
                }

                var user = UserConverter.FromSeedUser(userSeed);

                var createResult = await _userManager.CreateAsync(user, userSeed.Password);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("errors: {Message}", string.Join(", ", createResult.Errors.Select(x => x.Description)));
                    throw new Exception($"Failed to create user: {user.Email}");
                }

                var roles = new List<string> { "User" };
                if (user.Email == "superAdmin@gmail.com")
                {
                    roles.Add("Admin");
                    roles.Add("SuperAdmin");
                }
                var roleResult = await _userManager.AddToRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to assign role to user: {user.Email}");
                }
            }

            foreach (var permissionSeed in seedData.Permissions)
            {
                var permissionExists = await _permissionsRepository.ExistsByName(permissionSeed.Name);
                if (permissionExists)
                {
                    continue;
                }

                var permission = PermissionConverter.FromSeedPermission(permissionSeed);
 
                await _permissionsRepository.CreateAsync(permission);
            }
            await _unitOfWork.SaveChangesAsync();

            // * Seeding Permissions to the Roles

            var userRole = (await _rolesRepository.FindByNameWithPermissionsAsync(RolePolicies.User))!;
            var adminRole = (await _rolesRepository.FindByNameWithPermissionsAsync(RolePolicies.Admin))!;
            var superAdminRole = (await _rolesRepository.FindByNameWithPermissionsAsync(RolePolicies.SuperAdmin))!;
            GuardUtils.ThrowIfNull(userRole, nameof(userRole));
            GuardUtils.ThrowIfNull(adminRole, nameof(adminRole));
            GuardUtils.ThrowIfNull(superAdminRole, nameof(superAdminRole));

            var permissions = await _permissionsRepository.FindAllAsync();

            // checking for each permission if its default is set to a user/admin/superAdmin
            // if it is and its not already added then they are added to the role permissions collection
            userRole.Permissions
                .AddRangeIf(permissions, (perm) => perm.IsDefaultUser && !userRole.Permissions.Contains(perm));
            adminRole.Permissions
                .AddRangeIf(permissions, (perm) => perm.IsDefaultAdmin && !adminRole.Permissions.Contains(perm));
            superAdminRole.Permissions
                .AddRangeIf(permissions, (perm) => perm.IsDefaultSuperAdmin && !superAdminRole.Permissions.Contains(perm));

            // * ------------------ Restaurants related seeding ------------------
            // Permissions

            var restaurantPermissions = await _restaurantPermissionsRepository.FindAllAsync();
            foreach (var permissionSeed in seedData.RestaurantPermissions)
            {
                var permissionExists = await _restaurantPermissionsRepository
                .ExistsByMatchAsync(p => p.NormalizedName == permissionSeed.Name.ToUpperInvariant());

                if (permissionExists)
                {
                    continue;
                }

                var permission = RestaurantPermissionConverter.FromSeedPermission(permissionSeed);

                await _restaurantPermissionsRepository.CreateAsync(permission);
            }


            await _unitOfWork.CommitTransactionAsync(transaction);
            _logger.LogInformation("Exiting successfully...");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            await _unitOfWork.RollBackAsync(transaction);
            Environment.Exit(1);
            throw;
        }
    }

    public async Task SeedTenantRolesAsync(
        Guid restaurantId,
        Func<(
        RestaurantRole customerRole,
        RestaurantRole adminRole,
        RestaurantRole ownerRole), Task>? actionBeforeCommit = null)
    {
        _logger.LogInformation("Starting to seed restaurant {RestaurantId} roles and permissions...", restaurantId);
        var seeData = await DataLoader.GetSeedData();

        _tenantProvider.SetTenantId(restaurantId);

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            // creating roles
            List<RestaurantRole> createdRoles = [];
            foreach (var seedRole in seeData.RestaurantRoles)
            {
                var role = RestaurantRoleConverter.FromSeedRole(seedRole, restaurantId);

                var createdRole = await _restaurantRolesRepository.CreateAsync(role);
                createdRoles.Add(createdRole);
            }

            await _unitOfWork.SaveChangesAsync();

            var customerRole = createdRoles.Where(x => x.NormalizedName == RolePolicies.RestaurantCustomer).FirstOrDefault()!;
            var adminRole = createdRoles.Where(x => x.NormalizedName == RolePolicies.RestaurantAdmin).FirstOrDefault()!;
            var ownerRole = createdRoles.Where(x => x.NormalizedName == RolePolicies.RestaurantOwner).FirstOrDefault()!;
            GuardUtils.ThrowIfNull(customerRole);
            GuardUtils.ThrowIfNull(adminRole);
            GuardUtils.ThrowIfNull(ownerRole);

            // adding permissions to roles
            var permissions = await _restaurantPermissionsRepository.FindAllAsync();

            customerRole.Permissions.AddRangeIf(permissions, (perm) => perm.IsDefaultUser && !customerRole.Permissions.Contains(perm));
            adminRole.Permissions.AddRangeIf(permissions, (perm) => perm.IsDefaultAdmin && !adminRole.Permissions.Contains(perm));
            ownerRole.Permissions.AddRangeIf(permissions, (perm) => perm.IsDefaultOwner && !ownerRole.Permissions.Contains(perm));

            await _unitOfWork.SaveChangesAsync();

            if (actionBeforeCommit != null)
            {
                await actionBeforeCommit.Invoke((customerRole, adminRole, ownerRole));
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.CommitTransactionAsync(transaction);
            _logger.LogInformation("Restaurant roles and permissions were seeded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            await _unitOfWork.RollBackAsync(transaction);
        }
    }
}