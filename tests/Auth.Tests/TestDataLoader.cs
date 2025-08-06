using Auth.Data;
using Auth.Data.Seed;
using Auth.Models;
using Auth.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Utils;

namespace Auth.Tests;

public class TestDataLoader(AppDbContext ctx, IPasswordHasher<User> hasher)
{
    private readonly AppDbContext _ctx = ctx;
    private readonly IPasswordHasher<User> _hasher = hasher;
    public string userName = "john";
    public const string TestPassword = "123456";
    public SeedDataModel _jsonSeedData = default!;
    public Role AdminRole = default!;
    public Role UserRole = default!;
    public Role SuperAdminRole = default!;
    public List<User> Users = default!;
    public List<Role> Roles = default!;
    public List<Address> Addresses = default!;
    public List<Permission> NotSuperAdminOnlyPermissions { get; set; } = [];
    public List<Permission> Permissions { get; set; } = [];
    public const string SuperAdminEmail = "superAdmin@gmail.com";
    public const string SuperAdminEmailToTryDelete = "superAdminToDelete@gmail.com";
    public const string AdminEmailToTryDelete = "adminToDelete@gmail.com";
    public const string UserEmailToTryDelete = "userToDelete@gmail.com";
    public const string AdminEmail = "david.brown@gmail.com";
    public const string UserEmail = "emma.jones@gmail.com";
    public const string TestPermissionNameX1 = "TEST.PERMISSIONx1";
    public const string TestPermissionNameX2 = "TEST.PERMISSIONx2";
    private readonly Lock _initLock = new();
    private static bool _testDataInitialized = false;
    public Permission SuperAdminOnlyPermission { get; set; } = default!;
    private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    public async Task InitializeAsync()
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            if (_testDataInitialized)
            {
                return;
            }

            _jsonSeedData = await SeedingUtils.ParseJson<SeedDataModel>("./test-data.json");
            await LoadRolesAndPermissions();
            await LoadUsers();
            await LoadAddresses();

            _testDataInitialized = true;
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async Task CleanAsync(AppDbContext db)
    {
        var allUsers = await db.Users.ToListAsync();
        var allRoles = await db.Roles.ToListAsync();
        var allPermissions = await db.Permissions.ToListAsync();
        var allAddresses = await db.Addresses.ToListAsync();

        if (allAddresses.Any())
        {
            db.Addresses.RemoveRange(allAddresses);
        }

        if (allUsers.Any())
        {
            db.Users.RemoveRange(allUsers);
        }

        if (allPermissions.Any())
        {
            db.Permissions.RemoveRange(allPermissions);
        }

        if (allRoles.Any())
        {
            db.Roles.RemoveRange(allRoles);
        }

        await db.SaveChangesAsync();
    }
    private async Task<List<User>> LoadUsers()
    {
        var users = new List<User>();
        foreach (var jsonUser in _jsonSeedData.Users)
        {
            var user = new User
            {
                FirstName = jsonUser.FirstName,
                LastName = jsonUser.LastName,
                Email = jsonUser.Email,
                NormalizedEmail = jsonUser.Email.ToUpper(),
                UserName = jsonUser.Email,
                NormalizedUserName = jsonUser.Email.ToUpper(),
                EmailConfirmed = jsonUser.EmailConfirmed,
                PasswordHash = _hasher.HashPassword(null!, jsonUser.Password),
                IsGlobal = jsonUser.IsGlobal,
                RestaurantId = jsonUser.RestaurantId,
            };

            if (user.Email == SuperAdminEmail)
            {
                user.Roles.Add(SuperAdminRole);
                user.Roles.Add(AdminRole);
            }
            else if (user.Email == AdminEmail)
            {
                user.Roles.Add(AdminRole);
            }
            else if (user.Email == SuperAdminEmailToTryDelete)
            {
                user.Roles.Add(SuperAdminRole);
                user.Roles.Add(AdminRole);
            }
            else if (user.Email == AdminEmailToTryDelete) {
                user.Roles.Add(AdminRole);
            }

            user.Roles.Add(UserRole);

            users.Add(user);
        }

        await _ctx.Users.AddRangeAsync(users);
        await _ctx.SaveChangesAsync();

        Users = users;
        return users;
    }

    private async Task<List<Role>> LoadRolesAndPermissions()
    {
        var roles = new List<Role>();
        using var transaction = await _ctx.Database.BeginTransactionAsync();
        foreach (var jsonRole in _jsonSeedData.Roles)
        {
            var role = new Role
            {
                DisplayName = jsonRole.DisplayName,
                Name = jsonRole.Name,
                NormalizedName = jsonRole.Name.ToUpper()
            };

            roles.Add(role);
        }

        var permissions = new List<Permission>();
        foreach (var seedPerm in _jsonSeedData.Permissions)
        {
            permissions.Add(ConvertSeedToPermission(seedPerm));
        }
        await _ctx.Permissions.AddRangeAsync(permissions);

        await _ctx.Roles.AddRangeAsync(roles);
        await _ctx.SaveChangesAsync();

        UserRole = (await _ctx.Roles.Where(x => x.NormalizedName == RolePolicies.User).FirstOrDefaultAsync())!;
        AdminRole = (await _ctx.Roles.Where(x => x.NormalizedName == RolePolicies.Admin).FirstOrDefaultAsync())!;
        SuperAdminRole = (await _ctx.Roles.Where(x => x.NormalizedName == RolePolicies.SuperAdmin).FirstOrDefaultAsync())!;
        GuardUtils.ThrowIfNull(UserRole);
        GuardUtils.ThrowIfNull(AdminRole);
        GuardUtils.ThrowIfNull(SuperAdminRole);

        foreach (var perm in permissions)
        {
            if (perm.IsDefaultUser)
            {
                UserRole.Permissions.Add(perm);
            }

            if (perm.IsDefaultAdmin)
            {
                AdminRole.Permissions.Add(perm);
            }

            if (perm.IsDefaultSuperAdmin)
            {
                SuperAdminRole.Permissions.Add(perm);
            }

            if (SuperAdminOnlyPermission == null && SecurityUtils.IsSuperAdminOnly(perm))
            {
                SuperAdminOnlyPermission = perm;
            }

            if (!SecurityUtils.IsSuperAdminOnly(perm))
            {
                NotSuperAdminOnlyPermissions.Add(perm);
            }

            Permissions.Add(perm);
        } 
        
        await _ctx.SaveChangesAsync();
        await transaction.CommitAsync();

        Roles = await _ctx.Roles.Include(x => x.Permissions).ToListAsync();
        return roles;
    }

    private async Task<List<Address>> LoadAddresses()
    {
        var addresses = new List<Address>();
        var user = await _ctx.Users.Where(u => u.NormalizedEmail == UserEmail.ToUpper()).FirstOrDefaultAsync();
        foreach (var jsonAddress in _jsonSeedData.Addresses)
        {
            var address = new Address
            {
                City = jsonAddress.City,
                Country = jsonAddress.Country,
                AddressLine = jsonAddress.AddressLine,
                Latitude = jsonAddress.Latitude,
                Longitude = jsonAddress.Longitude,
                PostalCode = jsonAddress.PostalCode,
                State = jsonAddress.State,
                UserId = user!.Id,
            };

            addresses.Add(address);
        }

        await _ctx.Addresses.AddRangeAsync(addresses);
        await _ctx.SaveChangesAsync();

        Addresses = addresses;
        return addresses;
    }

    private static Permission ConvertSeedToPermission(SeedPermission seedPermission)
    {
        var permission = new Permission
        {
            Name = seedPermission.Name,
            DisplayName = seedPermission.DisplayName,
            IsDefaultUser = seedPermission.IsDefaultUser,
            IsDefaultAdmin = seedPermission.IsDefaultAdmin,
            IsDefaultSuperAdmin = seedPermission.IsDefaultSuperAdmin
        };

        return permission;
    }
}