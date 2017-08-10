using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace VstsDash.RestApi.Caching
{
    public class Cache : ICache
    {
        private readonly KeyedLock<object> _keyedLock = new KeyedLock<object>();

        private IMemoryCache LongTerm { get; } = CreateMemoryCache(TimeSpan.FromMinutes(30));

        private IMemoryCache MediumTerm { get; } = CreateMemoryCache(TimeSpan.FromMinutes(15));

        private IMemoryCache ShortTerm { get; } = CreateMemoryCache(TimeSpan.FromMinutes(5));

        private IMemoryCache VeryLongTerm { get; } = CreateMemoryCache(TimeSpan.FromHours(24));

        public async Task<TItem> GetOrCreateAsync<TItem>(
            object key,
            Func<Task<TItem>> factory,
            CacheDuration cacheDuration)
        {
            return await _keyedLock.RunWithLock(key, () => GetOrCreateAsyncInternal(key, factory, cacheDuration));
        }

        private static MemoryCache CreateMemoryCache(TimeSpan expiration)
        {
            return new MemoryCache(
                new MemoryCacheOptions
                {
                    ExpirationScanFrequency = expiration
                });
        }

        private static async Task<TItem> GetOrCreateAsync<TItem>(
            IMemoryCache cache,
            object key,
            Func<Task<TItem>> factory)
        {
            return await cache.GetOrCreateAsync(key, _ => factory());
        }

        private async Task<TItem> GetOrCreateAsyncInternal<TItem>(
            object key,
            Func<Task<TItem>> factory,
            CacheDuration cacheDuration)
        {
            switch (cacheDuration)
            {
                case CacheDuration.None: return await factory();
                case CacheDuration.Short: return await GetOrCreateAsync(ShortTerm, key, factory);
                case CacheDuration.Medium: return await GetOrCreateAsync(MediumTerm, key, factory);
                case CacheDuration.Long: return await GetOrCreateAsync(LongTerm, key, factory);
                case CacheDuration.VeryLong: return await GetOrCreateAsync(VeryLongTerm, key, factory);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(cacheDuration),
                        cacheDuration,
                        $"Value '{cacheDuration}' is not implemented.");
            }
        }
    }
}