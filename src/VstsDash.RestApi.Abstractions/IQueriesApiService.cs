using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    public interface IQueriesApiService
    {
        Task<QueryListApiResponse> GetList(string projectId);
    }
}