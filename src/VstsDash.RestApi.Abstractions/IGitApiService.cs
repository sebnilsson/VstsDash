using System;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    public interface IGitApiService
    {
        Task<GitBranchListApiResponse> GetBranchList(string repositoryId);

        Task<GitFullCommitApiResponse> GetCommit(string repositoryId, string commitId, int changeCount = 0);

        Task<GitCommitListApiResponse> GetCommitList(
            string repositoryId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string branch = null);

        Task<GitRepositoryApiResponse> GetRepository(string repositoryId, string projectId = null);

        Task<GitRepositoryListApiResponse> GetRepositoryList(string projectId = null);
    }
}