using System.Security.Cryptography.X509Certificates;
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
    public string testPassword = "123456";
    public SeedDataModel _cache;
    public Role AdminRole;
    public Role UserRole;
    public Role SuperAdminRole;
    public List<User> Users;
    public List<Role> Roles;

    public async Task<(List<User> users, List<Role> roles)> InitializeAsync()
    {
        _cache = await SeedingUtils.ParseJson<SeedDataModel>("./test-data.json");
        var roles = await LoadRoles();
        var users = await LoadUsers();

        return (users, roles);
    }

    public async Task CleanAsync(AppDbContext db)
    {
        db.Users.RemoveRange(Users);
    
        db.Roles.RemoveRange(Roles);
        
        await db.SaveChangesAsync();
    }

    private async Task<List<User>> LoadUsers()
    {
        var users = new List<User>();
        foreach (var jsonUser in _cache.Users)
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
                PasswordHash = _hasher.HashPassword(null, jsonUser.Password)
            };

            users.Add(user);
        }

        await _ctx.Users.AddRangeAsync(users);
        await _ctx.SaveChangesAsync();

        Users = users;
        return users;
    }

    private async Task<List<Role>> LoadRoles()
    {
        var roles = new List<Role>();
        foreach (var jsonRole in _cache.Roles)
        {
            var role = new Role
            {
                DisplayName = jsonRole.DisplayName,
                Name = jsonRole.Name
            };

            roles.Add(role);
        }

        await _ctx.Roles.AddRangeAsync(roles);
        await _ctx.SaveChangesAsync();

        UserRole = (await _ctx.Roles.Where(x => x.Name == RolePolicies.User).FirstOrDefaultAsync())!;
        AdminRole = (await _ctx.Roles.Where(x => x.Name == RolePolicies.Admin).FirstOrDefaultAsync())!;
        SuperAdminRole = (await _ctx.Roles.Where(x => x.Name == RolePolicies.SuperAdmin).FirstOrDefaultAsync())!;

        Roles = roles;
        return roles;
    }
}