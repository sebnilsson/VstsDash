using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.AppServices.WorkActivity
{
    public class WorkActivityAppService : IAppService
    {
        private const int DefaultActivityDays = 30;

        private static readonly TeamMemberApiResponse UnknownTeamMember = new TeamMemberApiResponse
                                                                          {
                                                                              DisplayName =
                                                                                  "[Unknown]",
                                                                              Id = Guid.Empty
                                                                          };

        private readonly ICache _cache;
        private readonly IGitApiService _gitApi;
        private readonly IIterationsApiService _iterationsApi;
        private readonly ITeamsApiService _teamsApi;
        private readonly WorkIterationAppService _workIterationAppService;

        public WorkActivityAppService(
            ICache cache,
            IGitApiService gitApi,
            IIterationsApiService iterationsApi,
            ITeamsApiService teamsApi,
            WorkIterationAppService workIterationAppService)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _gitApi = gitApi ?? throw new ArgumentNullException(nameof(gitApi));
            _iterationsApi = iterationsApi ?? throw new ArgumentNullException(nameof(iterationsApi));
            _teamsApi = teamsApi ?? throw new ArgumentNullException(nameof(teamsApi));
            _workIterationAppService = workIterationAppService
                                       ?? throw new ArgumentNullException(nameof(workIterationAppService));
        }

        public async Task<Activity> GetActivity(string projectId, string teamId, string iterationId)
        {
            var cacheKey = $"{GetType().FullName}.{nameof(GetActivity)}_{projectId}_{teamId}_{iterationId}";

            return await _cache.GetOrCreateAsync(
                       cacheKey,
                       () => GetActivityInternal(projectId, teamId, iterationId),
                       CacheDuration.Short);
        }

        private async Task<Activity> GetActivityInternal(string projectId, string teamId, string iterationId)
        {
            var iteration = await _iterationsApi.Get(projectId, teamId, iterationId);

            var fromDate = iteration?.Attributes?.StartDate ?? DateTime.Now;
            var toDate = iteration?.Attributes?.FinishDate ?? fromDate.AddDays(DefaultActivityDays);

            var capacitiesTask = _iterationsApi.GetCapacities(projectId, teamId, iterationId);
            var repositoriesTask = _gitApi.GetRepositoryList(projectId);
            var teamDaysOffTask = _iterationsApi.GetTeamDaysOff(projectId, teamId, iterationId);
            var teamMembersTask = _teamsApi.GetAllTeamMembers();

            var effortDoneTask =
                _workIterationAppService.GetWorkIterationDoneEffortsPerDay(projectId, teamId, iterationId);

            await Task.WhenAll(capacitiesTask, repositoriesTask, teamDaysOffTask, teamMembersTask, effortDoneTask);

            var capacities = capacitiesTask.Result;
            var repositories = repositoriesTask.Result;
            var teamDaysOff = teamDaysOffTask.Result;
            var teamMembers = teamMembersTask.Result;
            var effortDones = effortDoneTask.Result;

            var commitInfoTasks = repositories.Value.Select(
                    async x => await GetCommitInfo(x.Id, repositories.Value, teamMembers.Value, fromDate, toDate))
                .ToList();

            await Task.WhenAll(commitInfoTasks);

            var commitInfo = commitInfoTasks.SelectMany(x => x.Result).ToList();

            var teamCapacity = new TeamCapacity(iteration, teamDaysOff, teamMembers, capacities);

            return new Activity(commitInfo, fromDate, toDate, teamCapacity, effortDones, iteration);
        }

        private async Task<ICollection<CommitInfo>> GetCommitInfo(
            Guid repositoryId,
            IEnumerable<GitRepositoryApiResponse> repositories,
            IEnumerable<TeamMemberApiResponse> teamMembers,
            DateTime? fromDate,
            DateTime? toDate)
        {
            toDate = toDate?.AddDays(1);

            var branches = await _gitApi.GetBranchList(Convert.ToString(repositoryId));

            var commitsTasks = branches.BranchNames.Select(
                    async x => await _gitApi.GetCommitList(Convert.ToString(repositoryId), fromDate, toDate, x))
                .ToList();

            await Task.WhenAll(commitsTasks);

            var commits = commitsTasks.SelectMany(x => x.Result.Value)
                .Distinct(x => x.CommitId)
                .Where(x => x.Author.Email == x.Committer.Email)
                .ToList();

            var commitDetailsTasks = commits.Select(
                    async x => await _gitApi.GetCommit(Convert.ToString(repositoryId), x.CommitId, 1))
                .ToList();

            await Task.WhenAll(commitDetailsTasks);

            var commitDetails = commitDetailsTasks.Select(x => x.Result).ToList();

            return (from commit in commits
                    join t in teamMembers on commit.Author.Email equals t.UniqueName into t
                    join r in repositories on repositoryId equals r.Id into r
                    join c in commitDetails on commit.CommitId equals c.CommitId into c
                    from teamMember in t.DefaultIfEmpty(UnknownTeamMember)
                    from repository in r
                    from commitDetail in c.DefaultIfEmpty()
                    where commitDetail == null || commitDetail?.Parents.Count < 2
                          && (fromDate == null || commitDetail?.Author.Date >= fromDate)
                          && (toDate == null || commitDetail?.Author.Date < toDate)
                    select new CommitInfo(commitDetail ?? commit, teamMember, repository)).ToList();
        }
    }
}