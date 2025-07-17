namespace Shared.Contracts.Interfaces;

public interface IAuthProvider
{
    Task<IUserInfo> GetUserInfoAsync();
}