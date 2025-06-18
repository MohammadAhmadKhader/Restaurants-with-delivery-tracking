using Shared.Interfaces;

namespace Shared.Http.Clients;

public class AuthServiceClient : IAuthServiceClient
{
    public Task<object> LoginAsync()
    {
        throw new NotImplementedException();
    }

    public Task<object> RegisterAsync()
    {
        throw new NotImplementedException();
    }

    public Task<object> TestAsync(object data)
    {
        throw new NotImplementedException();
    }
}