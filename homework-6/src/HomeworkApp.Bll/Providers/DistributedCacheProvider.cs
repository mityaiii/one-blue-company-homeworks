using HomeworkApp.Bll.Providers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeworkApp.Bll.Providers;

public class DistributedCacheProvider : IDistributedCacheProvider
{
    private readonly IDistributedCache _distributedCache;

    public DistributedCacheProvider(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken token)
    {
        return await _distributedCache.GetStringAsync(key, token);
    }

    public async Task SetStringAsync(
        string key,
        string value,
        DistributedCacheEntryOptions options,
        CancellationToken token = default (CancellationToken))
    {
        await _distributedCache.SetStringAsync(key, value, options: options, token: token);
    }

    public async Task SetStringAsync(
        string key,
        string value,
        CancellationToken token = default(CancellationToken))
    {
        await _distributedCache.SetStringAsync(key, value, token: token);
    }
}