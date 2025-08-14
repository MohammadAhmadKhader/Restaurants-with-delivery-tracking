using Auth.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Auth.Models;
using Shared.Data.Patterns.UnitOfWork;
using Auth.Contracts.Dtos.Auth;
using Sprache;
using Shared.Exceptions;
using Auth.Data;
using Auth.Data.Seed;
using Restaurants.Contracts.Clients;
using Shared.Observability.Telemetry;

namespace Auth.Services;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IUsersService usersService,
    IRestaurantRolesService restaurantRolesService,
    IUnitOfWork<AppDbContext> unitOfWork,
    ITokenService tokenService,
    IPasswordHasher<User> bcrypt,
    IRestaurantServiceClient restaurantServiceClient,
    IDatabaseSeeder databaseSeeder) : IAuthService
{
    public const string resourceName = "restaurant";
    public async Task<(User, TokensResponse)> Login(LoginDto dto)
    {
        var user = await usersService.FindByEmailWithRolesAndPermissionsAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var signInResult = await ActivityUtils.WithEventAsync(
                "Starting CheckPasswordSignInAsync",
                "End CheckPasswordSignInAsync",
                () => signInManager.CheckPasswordSignInAsync(user, dto.Password, false));
    
        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var tokensResponse = await tokenService.GenerateTokensAsync(
            user.Id,
            user.Email!,
            [.. user.Roles.Select(r => r.NormalizedName)!]
        );

        return (user, tokensResponse);
    }

    public async Task<(User, TokensResponse)> LoginRestaurant(LoginDto dto)
    { 
        var user = await usersService.FindByEmailWithRestaurantRolesAndPermissionsAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var signInResult = await ActivityUtils.WithEventAsync(
                "Starting SignInManager.CheckPasswordSignInAsync",
                "End SignInManager.CheckPasswordSignInAsync",
                () => signInManager.CheckPasswordSignInAsync(user, dto.Password, false));

        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var tokensResponse = await tokenService.GenerateTokensAsync(
            user.Id,
            user.Email!,
            [..user.RestaurantRoles.Select(r => r.NormalizedName)!]
        );

        return (user, tokensResponse);
    }

    public async Task<(User, TokensResponse)> Register(RegisterDto dto)
    {
        var exists = await usersService.ExistsByEmailAsync(dto.Email);
        if (exists)
        {
            throw new InvalidOperationException($"An account with email '{dto.Email}' already exists.");
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            RestaurantId = null,
            IsGlobal = true,
        };

        await using var tx = await unitOfWork.BeginTransactionAsync();
        var userResult = await ActivityUtils.WithEventAsync(
                "Starting UserManager.CreateAsync",
                "End UserManager.CreateAsync",
                () => userManager.CreateAsync(user, dto.Password));
        if (!userResult.Succeeded)
        {
            await unitOfWork.RollBackAsync(tx);
            var errors = FormatIdentityErrors(userResult.Errors);
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        const string defaultRoleName = "USER";
        var roleResult = await ActivityUtils.WithEventAsync(
                "Starting UserManager.AddToRoleAsync",
                "End UserManager.AddToRoleAsync",
                () => userManager.AddToRoleAsync(user, defaultRoleName));
        if (!roleResult.Succeeded)
        {
            await unitOfWork.RollBackAsync(tx);
            var errors = FormatIdentityErrors(roleResult.Errors);
            throw new InvalidOperationException($"Adding role failed: {errors}");
        }


        var tokenResponse = await tokenService.GenerateTokensAsync(user.Id, user.Email, [defaultRoleName]);
        await unitOfWork.CommitTransactionAsync(tx);

        return (user, tokenResponse);
    }

    public async Task<(User, TokensResponse)> RegisterRestaurant(RegisterDto dto, Guid restaurantId)
    {
        var rest = await restaurantServiceClient.GetRestaurantByIdAsync(restaurantId);
        ResourceNotFoundException.ThrowIfNull(rest, resourceName, restaurantId);

        var userExists = await usersService.ExistsByEmailAsync(dto.Email);
        if (userExists)
        {
            throw new InvalidOperationException($"An account with email '{dto.Email}' already exists.");
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            RestaurantId = restaurantId,
            IsGlobal = false
        };

        await using var tx = await unitOfWork.BeginTransactionAsync();
        
       var userResult = await ActivityUtils.WithEventAsync(
                "Starting UserManager.CreateAsync",
                "End UserManager.CreateAsync",
                () => userManager.CreateAsync(user, dto.Password));
        if (!userResult.Succeeded)
        {
            await unitOfWork.RollBackAsync(tx);
            var errors = FormatIdentityErrors(userResult.Errors);
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        const string defaultRestaurantRoleName = "CUSTOMER";
        var role = await restaurantRolesService.FindByNameWithPermissionsAsync(defaultRestaurantRoleName);

        var tokenResponse = await tokenService.GenerateTokensAsync(
            user.Id,
            user.Email,
            [role!.NormalizedName]
        );
        await unitOfWork.CommitTransactionAsync(tx);

        return (user, tokenResponse);
    }

    public async Task ChangePassword(Guid userId, ResetPasswordDto dto)
    {
        var user = await usersService.FindByIdAsync(userId);
        ResourceNotFoundException.ThrowIfNull(user, UsersService.resourceName);

        var result = ActivityUtils.WithEvent(
            "Starting IPasswordHasher.VerifyHashedPassword",
            "End IPasswordHasher.VerifyHashedPassword",
            () => bcrypt.VerifyHashedPassword(user, user.PasswordHash!, dto.OldPassword));
            
        if (result == PasswordVerificationResult.Failed)
        {
            throw new AppValidationException("oldPassword", "Old password is incorrect");
        }

        user.PasswordHash = ActivityUtils.WithEvent(
            "Starting IPasswordHasher.VerifyHashedPassword",
            "End IPasswordHasher.VerifyHashedPassword",
            () => bcrypt.HashPassword(user, dto.NewPassword));

        await unitOfWork.SaveChangesAsync();
    }
    private static string FormatIdentityErrors(IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(e => e.Description));
    }

    public async Task<User> CreateRestaurantOwnerAndRoles(RegisterDto dto, Guid ownerId, Guid restaurantId)
    {
        var user = new User
        {
            Id = ownerId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            RestaurantId = restaurantId,
            IsGlobal = false
        };

        // the purpose is to apply user creation + roles creation in 1 transaction
        // * SaveChangesAsync is called intenrally after this action is called
        Func<(RestaurantRole, RestaurantRole, RestaurantRole), Task> actionBeforeCommit = async (roles) =>
        {
            var userResult = await ActivityUtils.WithEventAsync(
                    "Starting UserManager.CreateAsync",
                    "End UserManager.CreateAsync",
                    () => userManager.CreateAsync(user, dto.Password));

            if (!userResult.Succeeded)
            {
                var errors = FormatIdentityErrors(userResult.Errors);
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            var (customerRole, adminRole, ownerRole) = roles;

            user.RestaurantRoles.Add(customerRole);
            user.RestaurantRoles.Add(adminRole);
            user.RestaurantRoles.Add(ownerRole);
        };

        await databaseSeeder.SeedTenantRolesAsync(restaurantId, actionBeforeCommit);

        return user;
    }
}