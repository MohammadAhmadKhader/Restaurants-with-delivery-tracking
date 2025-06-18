namespace Shared.Interfaces;

public interface IAuthServiceClient
{
    Task<object> LoginAsync();
    Task<object> RegisterAsync();
    Task<object> TestAsync(object data);
}