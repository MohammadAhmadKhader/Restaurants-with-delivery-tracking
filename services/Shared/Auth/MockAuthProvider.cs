using Auth.Contracts.Dtos.Auth;
using Shared.Contracts.Interfaces;

namespace Shared.Auth;

public class MockAuthProvider : IAuthProvider
{
    public IUserInfo UserInfo => new UserInfo();
    public Task<IUserInfo> GetUserInfoAsync() => Task.FromResult(UserInfo)!;
}