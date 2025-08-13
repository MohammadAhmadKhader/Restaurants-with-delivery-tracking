using Auth.Models;

namespace Auth.Services.IServices;

public interface IPasswordResetTokensService
{
    Task<PasswordResetToken?> FindByTokenAsync(string token);
    Task<PasswordResetToken> CreateAsync(Guid userId);
    Task MarkAsUsedAsync(PasswordResetToken token);
}