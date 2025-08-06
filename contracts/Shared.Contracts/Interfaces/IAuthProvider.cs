namespace Shared.Contracts.Interfaces;

public interface IAuthProvider
{
    IUserInfo UserInfo { get; }
    Task<IUserInfo> GetUserInfoAsync();
}