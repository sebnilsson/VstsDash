using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VstsDash.RestApi
{
    public class StreamApiService : IStreamApiService, IApiService
    {
        private readonly IRestHttpClient _httpClient;

        public StreamApiService(IRestHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<HttpResponseMessage> GetResponse(string url)
        {
            ValidateUrlHost(url);

            return await _httpClient.HttpGet(url);
        }

        private static void ValidateUrlHost(string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (!uri.IsAbsoluteUri) return;

            if (!uri.Host.EndsWith(".visualstudio.com"))
                throw new ArgumentOutOfRangeException(nameof(url), "Host must be '.visualstudio.com'.");
        }
    }
}