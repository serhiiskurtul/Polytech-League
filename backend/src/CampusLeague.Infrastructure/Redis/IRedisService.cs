namespace CampusLeague.Infrastructure.Redis;

public interface IRedisService
{
    Task<T?> GetDataAsync<T>(string key);

    Task SetDataAsync<T>(string key, T data, TimeSpan expirationTime, string? group = null);

    Task RemoveDataAsync(string key);

    Task RemoveGroupAsync(string setName);
}
