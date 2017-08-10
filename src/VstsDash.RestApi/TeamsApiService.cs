using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.RestApi
{
    public class TeamsApiService : ITeamsApiService, IApiService
    {
        private readonly IRestApiClient _apiClient;

        private readonly IProjectsApiService _projectsApi;

        public TeamsApiService(IRestApiClient apiClient, IProjectsApiService projectsApi)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _projectsApi = projectsApi ?? throw new ArgumentNullException(nameof(projectsApi));
        }

        public async Task<TeamApiResponse> Get(string projectId, string teamId)
        {
            var url = $"/DefaultCollection/_apis/projects/{projectId}/teams/{teamId}?api-version=3.0";

            return await _apiClient.Get<TeamApiResponse>(url, CacheDuration.Medium);
        }

        public async Task<TeamMemberListApiResponse> GetAllTeamMembers()
        {
            return await GetAllTeamMembersInternal();
        }

        public async Task<TeamListApiResponse> GetList(string projectId)
        {
            var url = $"/DefaultCollection/_apis/projects/{projectId}/teams?api-version=3.0";

            return await _apiClient.Get<TeamListApiResponse>(url, CacheDuration.Medium);
        }

        public async Task<TeamMemberListApiResponse> GetMembers(string projectId, string teamId)
        {
            var url = $"/DefaultCollection/_apis/projects/{projectId}/teams/{teamId}/members?api-version=3.0";

            return await _apiClient.Get<TeamMemberListApiResponse>(url, CacheDuration.Medium);
        }

        private async Task<TeamMemberListApiResponse> GetAllTeamMembersInternal()
        {
            var projects = await _projectsApi.GetList();

            var teams = projects.Value.Select(x => Convert.ToString(x.Id))
                .Select(
                    x => new
                         {
                             ProjectId = x,
                             Task = GetList(x)
                         })
                .ToList();

            var teamTasks = teams.Select(x => x.Task);

            await Task.WhenAll(teamTasks);

            var teamIds = teams.SelectMany(
                x => x.Task.Result.Value.Select(
                    t => new
                         {
                             x.ProjectId,
                             TeamId = Convert.ToString(t.Id)
                         }));

            var teamMembersTasks = teamIds.Select(x => GetMembers(x.ProjectId, x.TeamId)).ToList();

            await Task.WhenAll(teamMembersTasks);

            var teamMembers = teamMembersTasks.SelectMany(x => x.Result.Value)
                .Distinct(x => x.Id)
                .OrderBy(x => x.DisplayName)
                .ThenByDescending(x => x.UniqueName)
                .ToList();

            return new TeamMemberListApiResponse
                   {
                       Count = teamMembers.Count,
                       Value = new ReadOnlyCollection<TeamMemberApiResponse>(teamMembers)
                   };
        }
    }
}