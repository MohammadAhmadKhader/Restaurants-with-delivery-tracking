using Auth.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Dtos.Auth;
using Sprache;
using Auth.Extensions;
using Shared.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace Auth.Services;

public class AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
 IUsersService usersService, IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher<User> bcrypt) : IAuthService
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

        var tokenResponse = await tokenService.GenerateTokenAsync(user.Id, user.Email!,
         [.. user.Roles.Select(r => r.Name)!], [.. user.Roles.SelectMany(r => r.Permissions).Select(r => r.Name)]);

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