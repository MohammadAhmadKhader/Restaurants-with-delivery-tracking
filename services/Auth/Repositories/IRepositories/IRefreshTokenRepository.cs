using Auth.Dtos;
using Auth.Dtos.Auth;

namespace Auth.Repositories.IRepositories;

public interface IRefreshTokenRepository
{
    Task StoreRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task<bool> RevokeRefreshTokenAsync(string token);
}