using Auth.Dtos;
using Auth.Services.IServices;
using Auth.Extensions.Mappers;
using Microsoft.AspNetCore.Identity;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Auth.Services;

class AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
 IUsersService usersService, IUnitOfWork unitOfWork, ITokenService tokenService) : IAuthService
{
    public async Task<(User, TokenResponse)> Login(LoginDto dto)
    {
        var user = await usersService.FindByEmailWithRolesAndPermissions(dto.Email!);
        if (user == null)
        {
            throw new InvalidOperationException($"User with email {dto.Email} was not found");
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var tokenResponse = await tokenService.GenerateTokenAsync(user.Id, user.Email, null, null);

        return (user, tokenResponse);
    }

    public async Task<(User, TokenResponse)> Register(RegisterDto dto)
    {
        var exists = await usersService.ExistsByEmail(dto.Email);
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

        using var tx = await unitOfWork.BeginTransactionAsync();
        var userResult = await userManager.CreateAsync(user, dto.Password);
        if (!userResult.Succeeded)
        {
            await unitOfWork.RollBackAsync(tx);
            var errors = FormatIdentityErrors(userResult.Errors);
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        var defaultRoleName = "User";
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

    private static string FormatIdentityErrors(IEnumerable<IdentityError> errors)
    {
        return string.Join(", ", errors.Select(e => e.Description));
    }
}