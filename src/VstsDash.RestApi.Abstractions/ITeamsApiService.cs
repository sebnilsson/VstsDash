using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    public interface ITeamsApiService
    {
        Task<TeamApiResponse> Get(string projectId, string teamId);

        Task<TeamMemberListApiResponse> GetAllTeamMembers();

        Task<TeamListApiResponse> GetList(string projectId);

        Task<TeamMemberListApiResponse> GetMembers(string projectId, string teamId);
    }
}