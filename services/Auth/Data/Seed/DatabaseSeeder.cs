using System.Text.Json;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace Auth.Data.Seed;

public class DatabaseSeeder: IDatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AppDbContext context, UserManager<User> userManager,
    RoleManager<Role> roleManager, IUnitOfWork unitOfWork, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task SeedAsync()
    {
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

            await _unitOfWork.CommitTransactionAsync(transaction);
        }
        catch(Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            await _unitOfWork.RollBackAsync(transaction);
            throw;
        }
    }
}