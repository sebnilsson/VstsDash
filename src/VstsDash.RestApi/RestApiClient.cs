using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.RestApi
{
    public class RestApiClient : IRestApiClient
    {
        private readonly string _baseUrl;

        private readonly ICache _cache;

        private readonly IRestHttpClient _httpClient;

        public RestApiClient(AppSettings appSettings, ICache cache, IRestHttpClient httpClient)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            _cache = cache ?? throw new ArgumentNullException(nameof(cache));

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            _baseUrl = $"https://{appSettings.Account}.visualstudio.com";
        }

        public async Task<T> Get<T>(string url, CacheDuration cacheDuration = CacheDuration.None)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var requestUri = GetRequestUri(url);

            return await _cache.GetOrCreateAsync(requestUri, () => GetInternal<T>(requestUri), cacheDuration);
        }

        public async Task<T> GetList<T, TValue>(
            string url,
            CacheDuration cacheDuration = CacheDuration.None,
            int? maxResultCount = null,
            int takeCount = 500) where T : ListApiResponseBase<TValue>
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var separator = url.Contains("?") ? "&" : "?";

            var model = await Get<T>($"{url}{separator}$top={takeCount}", cacheDuration);

            var modelCount = model.Count;

            var pageIndex = 1;

            while (modelCount >= takeCount && maxResultCount != null && maxResultCount < modelCount)
            {
                var skipCount = takeCount * pageIndex;

                var pagedUrl = $"{url}{separator}$skip={skipCount}";

                var pagedModel = await Get<T>(pagedUrl, cacheDuration);

                model.Count += pagedModel.Count;
                pagedModel.Value.ToList().ForEach(x => model.Value.Add(x));

                modelCount = pagedModel.Count;

                pageIndex++;
            }

            return model;
        }

        public async Task<T> Post<T>(string url, object json = null)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var requestUri = GetRequestUri(url);

            using (var httpResponse = await _httpClient.HttpPost(requestUri, json))
            {
                return await DeserializedResponse<T>(httpResponse);
            }
        }

        private static async Task<T> DeserializedResponse<T>(HttpResponseMessage httpResponse)
        {
            var httpContent = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                var message = $"Ensure success status code failed.{Environment.NewLine}{Environment.NewLine}"
                              + $"Response:{Environment.NewLine}{httpContent}";
                throw new Exception(message, ex);
            }

            var deserializedResponse = ApiResponseSerializationHelper.DeserializeResponse<T>(httpContent);
            return deserializedResponse;
        }

        private async Task<T> GetInternal<T>(string requestUri)
        {
            using (var httpResponse = await _httpClient.HttpGet(requestUri))
            {
                return await DeserializedResponse<T>(httpResponse);
            }
        }

        private string GetRequestUri(string url)
        {
            var requestUri = $"{_baseUrl}/{url.TrimStart('/')}";
            return requestUri;
        }
    }
}