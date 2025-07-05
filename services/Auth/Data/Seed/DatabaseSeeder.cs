using System.Text.Json;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Utils;
using Microsoft.AspNetCore.Identity;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Utils;

namespace Auth.Data.Seed;

public class DatabaseSeeder: IDatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUnitOfWork<AppDbContext> _unitOfWork;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IRolesRepository _rolesRepository;
    private readonly IPermissionsRepository _permissionsRepository;

    public DatabaseSeeder(
        AppDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IUnitOfWork<AppDbContext> unitOfWork,
        ILogger<DatabaseSeeder> logger,
        IRolesRepository rolesRepository,
        IPermissionsRepository permissionsRepository)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _rolesRepository = rolesRepository;
        _permissionsRepository = permissionsRepository;
        
    }
    public async Task SeedAsync()
    {
        if (!EnvironmentUtils.IsSeeding())
        {
            _logger.LogInformation("Skipping seeding");
            return;
        }

        _logger.LogInformation("Starting to seed data...");

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = await File.ReadAllTextAsync("./Data/seed.json");
        var seedData = JsonSerializer.Deserialize<SeedDataModel>(json, options);

        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var role in seedData!.Roles)
            {
                var rolesExists = await _roleManager.RoleExistsAsync(role.Name);
                if (rolesExists)
                {
                    continue;
                }

                var roleResult = await _roleManager.CreateAsync(new Role
                {
                    Name = role.Name,
                    DisplayName = role.DisplayName
                });

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

                var user = new User
                {
                    FirstName = userSeed.FirstName,
                    LastName = userSeed.LastName,
                    Email = userSeed.Email,
                    UserName = userSeed.Email,
                    EmailConfirmed = userSeed.EmailConfirmed
                };

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

                var permission = new Permission
                {
                    Name = permissionSeed.Name,
                    DisplayName = permissionSeed.DisplayName,
                    IsDefaultUser = permissionSeed.IsDefaultUser,
                    IsDefaultAdmin = permissionSeed.IsDefaultAdmin,
                    IsDefaultSuperAdmin = permissionSeed.IsDefaultSuperAdmin,
                };

                await _permissionsRepository.CreateAsync(permission);
            }
            await _unitOfWork.SaveChangesAsync();

            // * Seeding Permissions to the Roles

            var UserRole = await _rolesRepository.FindByNameWithPermissionsAsync(RolePolicies.User);
            var AdminRole = await _rolesRepository.FindByNameWithPermissionsAsync(RolePolicies.Admin);
            var SuperAdminRole = await _rolesRepository.FindByNameWithPermissionsAsync(RolePolicies.SuperAdmin);

            var permissions = await _permissionsRepository.FindAllAsync();

            // checking for each permission if its default is set to a user/admin/superAdmin
            // if it is and its not already added then they are added to the role permissions collection
            foreach (var perm in permissions)
            {
                if (perm.IsDefaultUser && !UserRole!.Permissions.Contains(perm))
                {
                    UserRole.Permissions.Add(perm);
                }
            }

            foreach (var perm in permissions)
            {
                if (perm.IsDefaultAdmin && !AdminRole!.Permissions.Contains(perm))
                {
                    AdminRole.Permissions.Add(perm);
                }
            }

            foreach (var perm in permissions)
            {
                if (perm.IsDefaultSuperAdmin && !SuperAdminRole!.Permissions.Contains(perm))
                {
                    SuperAdminRole.Permissions.Add(perm);
                }
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
}