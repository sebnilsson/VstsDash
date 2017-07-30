using System;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.RestApi
{
    public class GitApiService : IGitApiService, IApiService
    {
        private readonly IRestApiClient _apiClient;

        public GitApiService(IRestApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<GitBranchListApiResponse> GetBranchList(string repositoryId)
        {
            var url = $"DefaultCollection/_apis/git/repositories/{repositoryId}/refs/heads";

            return await _apiClient.Get<GitBranchListApiResponse>(url, CacheDuration.Long);
        }

        public async Task<GitFullCommitApiResponse> GetCommit(string repositoryId, string commitId, int changeCount = 0)
        {
            var query = "?" + (changeCount > 0 ? $"changeCount={changeCount}&" : null) + "api-version=3.0";
            var url = $"DefaultCollection/_apis/git/repositories/{repositoryId}/commits/{commitId}{query}";

            return await _apiClient.Get<GitFullCommitApiResponse>(url, CacheDuration.VeryLong);
        }

        public async Task<GitCommitListApiResponse> GetCommitList(
            string repositoryId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string branch = null)
        {
            var query = "?" + (fromDate != null ? $"fromDate={fromDate.Value:yyyy-MM-dd}&" : null)
                        + (toDate != null ? $"toDate={toDate.Value:yyyy-MM-dd}&" : null)
                        + (!string.IsNullOrWhiteSpace(branch) ? $"branch={branch}&" : null)
                        + "api-version=3.0";

            var url = $"DefaultCollection/_apis/git/repositories/{repositoryId}/commits{query}";

            return await _apiClient.GetList<GitCommitListApiResponse, GitCommitApiResponse>(url, CacheDuration.Short);
        }

        public async Task<GitRepositoryApiResponse> GetRepository(string repositoryId, string projectId = null)
        {
            var project = !string.IsNullOrWhiteSpace(projectId) ? $"{projectId}/" : null;

            var url = $"DefaultCollection/{project}_apis/git/repositories/{repositoryId}?api-version=3.0";

            return await _apiClient.Get<GitRepositoryApiResponse>(url, CacheDuration.VeryLong);
        }

        public async Task<GitRepositoryListApiResponse> GetRepositoryList(string projectId = null)
        {
            var project = !string.IsNullOrWhiteSpace(projectId) ? $"{projectId}/" : null;

            var url = $"DefaultCollection/{project}_apis/git/repositories?api-version=3.0";

            return await _apiClient.Get<GitRepositoryListApiResponse>(url, CacheDuration.Long);
        }
    }
}