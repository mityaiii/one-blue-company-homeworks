using Microsoft.Extensions.Caching.Distributed;

namespace HomeworkApp.Bll.Providers.Interfaces;

public interface IDistributedCacheProvider
{
    public Task<string?> GetStringAsync(string key, CancellationToken token);

    public Task SetStringAsync(string key,
        string value,
        DistributedCacheEntryOptions options,
        CancellationToken token = default (CancellationToken));

    public Task SetStringAsync(string key, string value, CancellationToken token = default(CancellationToken));
}