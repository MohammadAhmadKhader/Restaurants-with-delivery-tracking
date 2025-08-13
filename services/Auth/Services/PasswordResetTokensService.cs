using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Auth.Utils;
using Shared.Data.Patterns.UnitOfWork;

namespace Auth.Services;

public class PasswordResetTokensService(
    IPasswordResetTokensRepository passwordResetTokensRepository,
    IUnitOfWork<AppDbContext> unitOfWork
) : IPasswordResetTokensService
{
    private readonly IPasswordResetTokensRepository _passwordResetTokensRepository = passwordResetTokensRepository;
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;

    public async Task<PasswordResetToken> CreateAsync(Guid userId)
    {
        var newSecretToken = SecurityUtils.GenerateSecureToken();
        var token = new PasswordResetToken()
        {
            ExpiryDate = DateTime.UtcNow.AddMinutes(40),
            Token = newSecretToken,
            UserId = userId
        };

        var createdToken = await _passwordResetTokensRepository.CreateAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return createdToken;
    }

    public async Task<PasswordResetToken?> FindByTokenAsync(string token)
    {
        var foundToken = await _passwordResetTokensRepository.FindByMatchAsync(x => x.Token == token);
        if (foundToken == null || foundToken.IsUsed || foundToken.ExpiryDate < DateTime.UtcNow)
        {
            return null;
        }

        return foundToken;
    }

    public async Task MarkAsUsedAsync(PasswordResetToken token)
    {
        token.IsUsed = true;
        await _unitOfWork.SaveChangesAsync();
    }
}