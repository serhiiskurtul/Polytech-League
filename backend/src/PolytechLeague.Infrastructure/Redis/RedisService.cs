using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace PolytechLeague.Infrastructure.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;
    private readonly ILogger<RedisService> _logger;

    public RedisService(IConnectionMultiplexer db, ILogger<RedisService> logger)
    {
        _db = db.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetDataAsync<T>(string key)
    {
        if (!_db.Multiplexer.IsConnected)
        {
            _logger.LogWarning("Redis is not connected. Skipping GetData for key: {Key}", key);
            return default;
        }

        try
        {
            _logger.LogInformation("Fetching data from Redis for key: {Key}", key);
            var data = await _db.StringGetAsync(key);
            if (data.IsNullOrEmpty)
            {
                _logger.LogWarning("No data found in Redis for key: {Key}", key);
                return default;
            }

            _logger.LogInformation("Successfully fetched data from Redis for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(data!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data from Redis for key: {Key}", key);
            return default;
        }
    }

    public async Task SetDataAsync<T>(
        string key, T data, TimeSpan expirationTime, string? group = null)
    {
        if (!_db.Multiplexer.IsConnected)
        {
            _logger.LogWarning("Redis is not connected. Skipping SetData for key: {Key}", key);
            return;
        }

        try
        {
            _logger.LogInformation("Saving data to Redis for key: {Key} with expiration {Expiration}", key, expirationTime);

            var jsonData = JsonSerializer.Serialize(data);
            await _db.StringSetAsync(key, jsonData, expirationTime);

            if (!string.IsNullOrEmpty(group))
            {
                await _db.SetAddAsync(group, key);
                _logger.LogInformation("Key: {Key} added to group: {Group}", key, group);
            }

            _logger.LogInformation("Successfully saved data to Redis for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving data to Redis for key: {Key}", key);
        }
    }

    public async Task RemoveDataAsync(string key)
    {
        if (!_db.Multiplexer.IsConnected)
        {
            _logger.LogWarning("Redis is not connected. Skipping RemoveData for key: {Key}", key);
            return;
        }

        try
        {
            _logger.LogInformation("Removing data from Redis for key: {Key}", key);

            var result = await _db.KeyDeleteAsync(key);

            if (result)
                _logger.LogInformation("Successfully removed data from Redis for key: {Key}", key);
            else
                _logger.LogWarning("No data found in Redis to remove for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing data from Redis for key: {Key}", key);
        }
    }

    public async Task RemoveGroupAsync(string setName)
    {
        if (!_db.Multiplexer.IsConnected)
        {
            _logger.LogWarning("Redis is not connected. Skipping RemoveGroup for group: {Group}", setName);
            return;
        }

        try
        {
            _logger.LogInformation("Removing group: {Group} from Redis", setName);

            var keys = await _db.SetMembersAsync(setName);

            foreach (var key in keys)
            {
                await _db.KeyDeleteAsync(key.ToString());
                _logger.LogInformation("Removed key: {Key} from Redis group: {Group}", key, setName);
            }

            await _db.KeyDeleteAsync(setName);
            _logger.LogInformation("Successfully removed group: {Group} and all its keys from Redis", setName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing group: {Group} from Redis", setName);
        }
    }
}
