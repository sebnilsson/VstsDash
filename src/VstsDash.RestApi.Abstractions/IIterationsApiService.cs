using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    public interface IIterationsApiService
    {
        Task<IterationApiResponse> Get(string projectId, string teamId, string iterationId);

        Task<IterationCapacityListApiResponse> GetCapacities(string projectId, string teamId, string iterationId);

        Task<IterationListApiResponse> GetList(string projectId, string teamId);

        Task<IterationDaysOffApiResponse> GetTeamDaysOff(string projectId, string teamId, string iterationId);
    }
}