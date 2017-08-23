using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace VstsDash.RestApi.Caching
{
    public class Cache : ICache
    {
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private readonly KeyedLock<object> _keyedLock = new KeyedLock<object>();

        public async Task<TItem> GetOrCreateAsync<TItem>(
            object key,
            Func<Task<TItem>> factory,
            CacheDuration cacheDuration)
        {
            return await _keyedLock.RunWithLock(key, () => GetOrCreateAsyncInternal(key, factory, cacheDuration));
        }

        private static TimeSpan GetRelativeExpiration(CacheDuration cacheDuration)
        {
            switch (cacheDuration)
            {
                case CacheDuration.None:
                    return TimeSpan.FromSeconds(0);
                case CacheDuration.Short:
                    return TimeSpan.FromMinutes(5);
                case CacheDuration.Medium:
                    return TimeSpan.FromMinutes(15);
                case CacheDuration.Long:
                    return TimeSpan.FromMinutes(30);
                case CacheDuration.VeryLong:
                    return TimeSpan.FromDays(365);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(cacheDuration),
                        cacheDuration,
                        $"Value '{cacheDuration}' is not implemented.");
            }
        }

        private async Task<TItem> GetOrCreateAsyncInternal<TItem>(
            object key,
            Func<Task<TItem>> factory,
            CacheDuration cacheDuration)
        {
            TItem cacheEntry;

            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                cacheEntry = await factory();

                var relativeExpiration = GetRelativeExpiration(cacheDuration);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(relativeExpiration);

                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}