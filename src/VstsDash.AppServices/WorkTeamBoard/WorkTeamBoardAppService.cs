using System;
using System.Threading.Tasks;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi;

namespace VstsDash.AppServices.WorkTeamBoard
{
    public class WorkTeamBoardAppService : IAppService
    {
        private readonly IIterationsApiService _iterationsApi;

        private readonly ITeamsApiService _teamsApi;

        private readonly WorkIterationAppService _workIterationAppService;

        public WorkTeamBoardAppService(
            IIterationsApiService iterationsApi,
            ITeamsApiService teamsApi,
            WorkIterationAppService workIterationAppService)
        {
            _iterationsApi = iterationsApi ?? throw new ArgumentNullException(nameof(iterationsApi));
            _teamsApi = teamsApi ?? throw new ArgumentNullException(nameof(teamsApi));
            _workIterationAppService = workIterationAppService
                                       ?? throw new ArgumentNullException(nameof(workIterationAppService));
        }

        public async Task<TeamBoard> GetTeamBoard(string projectId, string teamId, string iterationId)
        {
            var capacitiesTask = _iterationsApi.GetCapacities(projectId, teamId, iterationId);
            var iterationTask = _iterationsApi.Get(projectId, teamId, iterationId);
            var teamDaysOffTask = _iterationsApi.GetTeamDaysOff(projectId, teamId, iterationId);
            var teamMembersTask = _teamsApi.GetAllTeamMembers();
            var workIterationTask = _workIterationAppService.GetWorkIteration(projectId, teamId, iterationId);

            await Task.WhenAll(iterationTask, capacitiesTask, teamDaysOffTask, teamMembersTask, workIterationTask);

            var capaticies = capacitiesTask.Result;
            var iteration = iterationTask.Result;
            var teamDaysOff = teamDaysOffTask.Result;
            var teamMembers = teamMembersTask.Result;
            var workIteration = workIterationTask.Result;

            return new TeamBoard(teamMembers, iteration, capaticies, teamDaysOff, workIteration);
        }
    }
}