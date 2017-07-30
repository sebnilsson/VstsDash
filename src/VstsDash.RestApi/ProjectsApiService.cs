using System;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.RestApi
{
    public class ProjectsApiService : IProjectsApiService, IApiService
    {
        private readonly IRestApiClient _apiClient;

        public ProjectsApiService(IRestApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<ProjectApiResponse> Get(string projectId)
        {
            var url = $"DefaultCollection/_apis/projects/{projectId}?api-version=3.0";

            return await _apiClient.Get<ProjectApiResponse>(url, CacheDuration.Long);
        }

        public async Task<ProjectListApiResponse> GetList()
        {
            const string url = "DefaultCollection/_apis/projects?api-version=3.0";

            return await _apiClient.Get<ProjectListApiResponse>(url, CacheDuration.Long);
        }
    }
}