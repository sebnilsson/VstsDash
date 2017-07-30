using System.Net.Http;
using System.Threading.Tasks;

namespace VstsDash.RestApi
{
    public interface IStreamApiService
    {
        Task<HttpResponseMessage> GetResponse(string url);
    }
}