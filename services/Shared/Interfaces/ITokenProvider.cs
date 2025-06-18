namespace Shared.Interfaces;

public interface ITokenProvider
{
    string? GetToken();
    string? GetAuthorizationHeader();
}