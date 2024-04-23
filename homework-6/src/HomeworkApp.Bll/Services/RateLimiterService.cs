using HomeworkApp.Bll.Providers.Interfaces;
using HomeworkApp.Bll.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeworkApp.Bll.Services;

public class RateLimiterService : IRateLimiterService
{
    private const int Limit = 100;
    private readonly IDistributedCacheProvider _distributedCache;
    private readonly TimeSpan _period;
    
    public RateLimiterService(IDistributedCacheProvider distributedCache)
    {
        _distributedCache = distributedCache;
        _period = TimeSpan.FromSeconds(60);
    }

    public async Task<bool> IsAllowed(string clientId, CancellationToken token)
    {
        var cacheKey = $"rate_limit:{clientId}";
        var cacheEntry = await _distributedCache.GetStringAsync(cacheKey, token: token);

        if (cacheEntry == null)
        {
            await _distributedCache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _period
            }, token: token);
            return true;
        }

        int requestsCount = int.Parse(cacheEntry) + 1;

        if (requestsCount > Limit)
        {
            return false;
        }

        await _distributedCache.SetStringAsync(cacheKey, requestsCount.ToString(), token: token);
        return true;
    }
}