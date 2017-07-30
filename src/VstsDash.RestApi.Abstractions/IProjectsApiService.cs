using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    public interface IProjectsApiService
    {
        Task<ProjectApiResponse> Get(string projectId);

        Task<ProjectListApiResponse> GetList();
    }
}