using System;
using System.Threading.Tasks;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi;
using VstsDash.RestApi.Caching;

namespace VstsDash.AppServices.WorkStories
{
    public class WorkStoriesAppService : IAppService
    {
        private const int DefaultActivityDays = 30;
        private readonly ICache _cache;
        private readonly IIterationsApiService _iterationsApi;
        private readonly ITeamsApiService _teamsApi;
        private readonly WorkIterationAppService _workIterationAppService;

        public WorkStoriesAppService(
            ICache cache,
            IIterationsApiService iterationsApi,
            ITeamsApiService teamsApi,
            WorkIterationAppService workIterationAppService)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _iterationsApi = iterationsApi ?? throw new ArgumentNullException(nameof(iterationsApi));
            _teamsApi = teamsApi ?? throw new ArgumentNullException(nameof(teamsApi));
            _workIterationAppService = workIterationAppService ??
                                       throw new ArgumentNullException(nameof(workIterationAppService));
        }

        public async Task<WorkStories> GetStories(string projectId, string teamId, string iterationId)
        {
            var cacheKey = $"{GetType().FullName}.{nameof(GetStories)}_{projectId}_{teamId}_{iterationId}";

            return await _cache.GetOrCreateAsync(cacheKey, () => GetStoriesInternal(projectId, teamId, iterationId),
                CacheDuration.Short);
        }

        private async Task<WorkStories> GetStoriesInternal(string projectId, string teamId, string iterationId)
        {
            var iteration = await _iterationsApi.Get(projectId, teamId, iterationId);

            var fromDate = iteration?.Attributes?.StartDate ?? DateTime.Now;
            var toDate = iteration?.Attributes?.FinishDate ?? fromDate.AddDays(DefaultActivityDays);

            var capacitiesTask = _iterationsApi.GetCapacities(projectId, teamId, iterationId);
            var teamDaysOffTask = _iterationsApi.GetTeamDaysOff(projectId, teamId, iterationId);
            var teamMembersTask = _teamsApi.GetAllTeamMembers();
            var workIterationTask = _workIterationAppService.GetWorkIteration(projectId, teamId, iterationId);

            await Task.WhenAll(capacitiesTask, teamDaysOffTask, teamMembersTask);

            var capacities = capacitiesTask.Result;
            var teamDaysOff = teamDaysOffTask.Result;
            var teamMembers = teamMembersTask.Result;
            var workIteration = workIterationTask.Result;

            var teamCapacity = new TeamCapacity(iteration, teamDaysOff, teamMembers, capacities);

            return new WorkStories(fromDate, toDate, teamCapacity, iteration, workIteration);
        }
    }
}