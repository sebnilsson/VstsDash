using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.RestApi
{
    public interface IRestApiClient
    {
        Task<T> Get<T>(string url, CacheDuration cacheDuration = CacheDuration.None);

        Task<T> GetList<T, TValue>(
            string url,
            CacheDuration cacheDuration = CacheDuration.None,
            int? maxResultCount = null,
            int takeCount = 500)
            where T : ListApiResponseBase<TValue>;

        Task<T> Post<T>(string url, object json = null);
    }
}