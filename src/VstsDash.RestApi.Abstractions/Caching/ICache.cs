using System;
using System.Threading.Tasks;

namespace VstsDash.RestApi.Caching
{
    public interface ICache
    {
        Task<TItem> GetOrCreateAsync<TItem>(object key, Func<Task<TItem>> factory, CacheDuration cacheDuration);
    }
}