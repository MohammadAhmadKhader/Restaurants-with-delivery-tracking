namespace Shared.Contracts.Interfaces;

public interface ITokenProvider
{
    string? GetToken();
    string? GetAuthorizationHeader();
}