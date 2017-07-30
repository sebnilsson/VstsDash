using System;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.RestApi
{
    public class IterationsApiService : IIterationsApiService, IApiService
    {
        private readonly IRestApiClient _apiClient;

        public IterationsApiService(IRestApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<IterationApiResponse> Get(string projectId, string teamId, string iterationId)
        {
            var url =
                $"DefaultCollection/{projectId}/{teamId}/_apis/work/TeamSettings/Iterations/{iterationId}?api-version=3.0";

            return await _apiClient.Get<IterationApiResponse>(url, CacheDuration.Medium);
        }

        public async Task<IterationCapacityListApiResponse> GetCapacities(
            string projectId,
            string teamId,
            string iterationId)
        {
            var url =
                $"DefaultCollection/{projectId}/{teamId}/_apis/work/TeamSettings/Iterations/{iterationId}/Capacities?api-version=3.0";

            return await _apiClient.Get<IterationCapacityListApiResponse>(url, CacheDuration.Medium);
        }

        public async Task<IterationListApiResponse> GetList(string projectId, string teamId)
        {
            var url =
                $"DefaultCollection/{projectId}/{teamId}/_apis/work/TeamSettings/Iterations?api-version=3.0";

            return await _apiClient.Get<IterationListApiResponse>(url, CacheDuration.Medium);
        }

        public async Task<IterationDaysOffApiResponse> GetTeamDaysOff(string projectId, string teamId,
            string iterationId)
        {
            var url =
                $"DefaultCollection/{projectId}/{teamId}/_apis/work/TeamSettings/Iterations/{iterationId}/TeamDaysOff?api-version=3.0";

            return await _apiClient.Get<IterationDaysOffApiResponse>(url, CacheDuration.Medium);
        }
    }
}