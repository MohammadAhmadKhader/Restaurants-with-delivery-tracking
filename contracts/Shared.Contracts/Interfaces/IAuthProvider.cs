namespace Shared.Contracts.Interfaces;

public interface IAuthProvider
{
    Task<IUserClaims> GetUserClaimsAsync();
}