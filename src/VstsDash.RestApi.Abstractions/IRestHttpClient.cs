using System.Net.Http;
using System.Threading.Tasks;

namespace VstsDash.RestApi
{
    public interface IRestHttpClient
    {
        Task<HttpResponseMessage> HttpGet(string requestUri);

        Task<HttpResponseMessage> HttpPost(string requestUri, object json = null);
    }
}