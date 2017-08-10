using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VstsDash.RestApi
{
    public class RestHttpClient : IRestHttpClient
    {
        private static readonly MediaTypeWithQualityHeaderValue AcceptApplicationJsonHeader =
            new MediaTypeWithQualityHeaderValue("application/json");

        private readonly AuthenticationHeaderValue _basicAuthorizationHeader;

        public RestHttpClient(AppSettings appSettings)
        {
            var basicAuthorizationHeaderValue = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(string.Format("{0}:{1}", string.Empty, appSettings.AccessToken)));

            _basicAuthorizationHeader = new AuthenticationHeaderValue("Basic", basicAuthorizationHeaderValue);
        }

        public async Task<HttpResponseMessage> HttpGet(string requestUri)
        {
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            using (var client = CreateHttpClient())
            {
                Debug.WriteLine($"HttpClient GET '{requestUri}'.");

                return await client.GetAsync(requestUri);
            }
        }

        public async Task<HttpResponseMessage> HttpPost(string requestUri, object json = null)
        {
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));

            var content = json != null ? ApiResponseSerializationHelper.SerializeRequest(json) : null;
            var contentType = json != null ? "application/json" : null;

            using (var client = CreateHttpClient())
            {
                var postContent = new StringContent(content ?? string.Empty, Encoding.UTF8, contentType);

                Debug.WriteLine($"HttpClient POST '{requestUri}'.");

                return await client.PostAsync(requestUri, postContent);
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(AcceptApplicationJsonHeader);
            client.DefaultRequestHeaders.Authorization = _basicAuthorizationHeader;

            return client;
        }
    }
}