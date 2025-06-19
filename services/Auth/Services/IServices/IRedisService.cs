namespace Auth.Services.IServices;
public interface IRedisService
{
    Task SetStringAsync(string key, string value);
    Task<string?> GetStringAsync(string key);
}