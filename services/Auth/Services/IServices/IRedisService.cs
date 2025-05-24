using Auth.Dtos;

namespace Auth.Repositories.IRepositories;

public interface IRedisService
{
    Task SetStringAsync(string key, string value);
    Task<string?> GetStringAsync(string key);
}