using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace MiniCRM.Shared.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly IServer _server;
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions Opts = new() { PropertyNameCaseInsensitive = true };

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _db     = redis.GetDatabase();
        _server = redis.GetServer(redis.GetEndPoints()[0]);
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (!value.HasValue) return default;
            _logger.LogDebug("Cache HIT: {Key}", key);
            return JsonSerializer.Deserialize<T>(value!, Opts);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache GET failed for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, Opts);
            await _db.StringSetAsync(key, json, expiry);
            _logger.LogDebug("Cache SET: {Key} (expires in {Expiry}s)", key, expiry.TotalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache SET failed for key {Key}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        try
        {
            var keys = _server.Keys(pattern: $"{prefix}*").ToArray();
            if (keys.Length > 0)
            {
                await _db.KeyDeleteAsync(keys);
                _logger.LogDebug("Cache INVALIDATED {Count} keys with prefix {Prefix}", keys.Length, prefix);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache REMOVE failed for prefix {Prefix}", prefix);
        }
    }
}
