using Auth.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Auth.Models;
using Shared.Data.Patterns.UnitOfWork;
using Auth.Contracts.Dtos.Auth;
using Sprache;
using Shared.Exceptions;
using Auth.Data;

namespace Auth.Services;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IUsersService usersService,
    IRestaurantRolesService restaurantRolesService,
    IUnitOfWork<AppDbContext> unitOfWork,
    ITokenService tokenService,
    IPasswordHasher<User> bcrypt) : IAuthService
{
    public async Task<(User, TokenResponse)> Login(LoginDto dto)
    {
        var user = await usersService.FindByEmailWithRolesAndPermissionsAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var tokenResponse = await tokenService.GenerateTokenAsync(
            user.Id,
            user.Email!,
            [..user.Roles.Select(r => r.Name)!],
            [..user.Roles.SelectMany(r => r.Permissions).Select(r => r.Name)]
        );

        return (user, tokenResponse);
    }

    public async Task<(User, TokenResponse)> LoginRestaurant(LoginDto dto)
    { 
        var user = await usersService.FindByEmailWithRestaurantRolesAndPermissionsAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var tokenResponse = await tokenService.GenerateTokenAsync(
            user.Id,
            user.Email!,
            [..user.RestaurantRoles.Select(r => r.NormalizedName)!],
            [..user.RestaurantRoles.SelectMany(r => r.Permissions).Select(r => r.NormalizedName)]
        );

        return (user, tokenResponse);
    }

    public async Task<(User, TokenResponse)> Register(RegisterDto dto)
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
        var userResult = await userManager.CreateAsync(user, dto.Password);
        if (!userResult.Succeeded)
        {
            await unitOfWork.RollBackAsync(tx);
            var errors = FormatIdentityErrors(userResult.Errors);
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        const string defaultRoleName = "USER";
        var roleResult = await userManager.AddToRoleAsync(user, defaultRoleName);
        if (!roleResult.Succeeded)
        {
            await unitOfWork.RollBackAsync(tx);
            var errors = FormatIdentityErrors(roleResult.Errors);
            throw new InvalidOperationException($"Adding role failed: {errors}");
        }


        var tokenResponse = await tokenService.GenerateTokenAsync(user.Id, user.Email, [defaultRoleName], []);
        await unitOfWork.CommitTransactionAsync(tx);

        return (user, tokenResponse);
    }

    public async Task<(User, TokenResponse)> RegisterRestaurant(RegisterDto dto, Guid restaurantId)
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
            RestaurantId = restaurantId,
            IsGlobal = false
        };

        await using var tx = await unitOfWork.BeginTransactionAsync();
        var userResult = await userManager.CreateAsync(user, dto.Password);
        if (!userResult.Succeeded)
        {
            await unitOfWork.RollBackAsync(tx);
            var errors = FormatIdentityErrors(userResult.Errors);
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        const string defaultrestaurantRoleName = "CUSTOMER";
        var role = await restaurantRolesService.FindByNameWithPermissionsAsync(defaultrestaurantRoleName);

        var tokenResponse = await tokenService.GenerateTokenAsync(
            user.Id,
            user.Email,
            [role!.NormalizedName],
            [..role!.Permissions.Select(p => p.NormalizedName)]
        );
        await unitOfWork.CommitTransactionAsync(tx);

        return (user, tokenResponse);
    }

    public async Task ChangePassword(Guid userId, ResetPasswordDto dto)
    {
        var user = await usersService.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ResourceNotFoundException("user");
        }

        var result = bcrypt.VerifyHashedPassword(user, user.PasswordHash!, dto.OldPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new AppValidationException("oldPassword", "Old password is incorrect");
        }

        user.PasswordHash = bcrypt.HashPassword(user, dto.NewPassword);
        await unitOfWork.SaveChangesAsync();
    }
    private static string FormatIdentityErrors(IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(e => e.Description));
    }
}