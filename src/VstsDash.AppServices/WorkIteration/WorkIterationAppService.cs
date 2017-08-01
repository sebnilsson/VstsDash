using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VstsDash.RestApi;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class WorkIterationAppService : IAppService
    {
        private readonly IIterationsApiService _iterationsApi;
        private readonly ITeamsApiService _teamsApi;
        private readonly IWiqlApiService _wiqlApi;
        private readonly IWorkApiService _workApi;

        public WorkIterationAppService(
            IIterationsApiService iterationsApi,
            ITeamsApiService teamsApi,
            IWiqlApiService wiqlApi,
            IWorkApiService workApi)
        {
            _iterationsApi = iterationsApi ?? throw new ArgumentNullException(nameof(iterationsApi));
            _teamsApi = teamsApi ?? throw new ArgumentNullException(nameof(teamsApi));
            _wiqlApi = wiqlApi ?? throw new ArgumentNullException(nameof(wiqlApi));
            _workApi = workApi ?? throw new ArgumentNullException(nameof(workApi));
        }

        public async Task<Iteration> GetWorkIteration(string projectId, string teamId, string iterationId)
        {
            var iterationTask = _iterationsApi.Get(projectId, teamId, iterationId);
            var settingTask = _workApi.GetSetting(projectId, teamId);
            var teamMembersTask = _teamsApi.GetAllTeamMembers();

            await Task.WhenAll(iterationTask, settingTask, teamMembersTask);

            var iteration = iterationTask.Result;
            var setting = settingTask.Result;
            var teamMembers = teamMembersTask.Result.Value.ToList();

            var isBacklog = iteration.Id == setting.BacklogIteration?.Id;

            var capacities = !isBacklog
                ? await _iterationsApi.GetCapacities(projectId, teamId, iterationId)
                : new IterationCapacityListApiResponse();

            var workItems = await GetWorkIterationWorkItems(projectId, iteration.Path, isBacklog);

            var result = new Iteration(workItems, teamMembers, iteration, capacities, isBacklog);
            return result;
        }

        public async Task<IReadOnlyDictionary<DateTime, double?>> GetWorkIterationDoneEffortsPerDay(
            string projectId,
            string teamId,
            string iterationId)
        {
            var workIterationItemsTask = GetWorkIteration(projectId, teamId, iterationId);
            var iterationTask = _iterationsApi.Get(projectId, teamId, iterationId);
            var teamDaysOffTask = _iterationsApi.GetTeamDaysOff(projectId, teamId, iterationId);

            await Task.WhenAll(workIterationItemsTask, iterationTask);

            var iteration = iterationTask.Result;
            var teamDaysOff = teamDaysOffTask.Result;
            var workIterationItems = workIterationItemsTask.Result.Items;

            var iterationCapacity = new IterationCapacity(iteration, teamDaysOff);

            var doneEffortsPerDay = GetWorkIterationDoneEffortsPerDayInternal(workIterationItems, iterationCapacity)
                .ToDictionary(x => x.Key, x => x.Value);
            return new ReadOnlyDictionary<DateTime, double?>(doneEffortsPerDay);
        }

        private static IEnumerable<KeyValuePair<DateTime, double?>> GetWorkIterationDoneEffortsPerDayInternal(
            ICollection<WorkItem> workIterationItems,
            IterationCapacity iterationCapacity)
        {
            foreach (var workDay in iterationCapacity.NetWorkDays)
            {
                var isWorkDayPast = workDay <= DateTime.Today;

                var doneWorkIterationItems = workIterationItems
                    .Where(x =>
                        x.IsStateDone && x.Effort > 0 &&
                        (x.ClosedDate ?? x.ChangedDate ?? DateTime.MaxValue).Date <= workDay.Date)
                    .ToList();

                var doneEffortSum = isWorkDayPast
                    ? (doneWorkIterationItems.Any() ? doneWorkIterationItems.Sum(x => x.Effort) : 0)
                    : (double?)null;
                yield return new KeyValuePair<DateTime, double?>(workDay, doneEffortSum);
            }
        }

        public async Task<WorkItemListApiResponse> GetWorkIterationWorkItems(
            string projectId,
            string iterationPath,
            bool isBacklog = false)
        {
            var wiqlWorkItems = await _wiqlApi.QueryWorkItems(projectId, iterationPath, isBacklog);

            var workItemSourceIds = wiqlWorkItems.WorkItemRelations.Select(x => x.Source?.Id);
            var workItemTargetIds = wiqlWorkItems.WorkItemRelations.Select(x => x.Target?.Id);

            var workItemIds =
                workItemSourceIds.Concat(workItemTargetIds)
                    .Where(x => x != null && x > 0)
                    .Select(x => x.Value)
                    .Distinct()
                    .ToArray();

            var workItems = await _workApi.GetWorkItemList(workItemIds);
            return workItems;
        }
    }
}