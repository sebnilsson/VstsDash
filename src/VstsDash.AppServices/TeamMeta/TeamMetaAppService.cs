using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VstsDash.RestApi;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.AppServices.TeamMeta
{
    public class TeamMetaAppService
    {
        private readonly ICache _cache;
        private readonly IGitApiService _gitApi;
        private readonly IIterationsApiService _iterationsApi;
        private readonly IProjectsApiService _projectsApi;
        private readonly IQueriesApiService _queriesApi;
        private readonly ITeamsApiService _teamsApi;
        private readonly IWorkApiService _workApi;

        public TeamMetaAppService(
            ICache cache,
            IGitApiService gitApi,
            IIterationsApiService iterationsApi,
            IProjectsApiService projectsApi,
            IQueriesApiService queriesApi,
            ITeamsApiService teamsApi,
            IWorkApiService workApi)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _gitApi = gitApi ?? throw new ArgumentNullException(nameof(gitApi));
            _iterationsApi = iterationsApi ?? throw new ArgumentNullException(nameof(iterationsApi));
            _projectsApi = projectsApi ?? throw new ArgumentNullException(nameof(projectsApi));
            _queriesApi = queriesApi ?? throw new ArgumentNullException(nameof(queriesApi));
            _teamsApi = teamsApi ?? throw new ArgumentNullException(nameof(teamsApi));
            _workApi = workApi ?? throw new ArgumentNullException(nameof(workApi));
        }

        public async Task<TeamMetaResult> GetTeamMeta()
        {
            var cacheKey = $"{GetType().FullName}.{nameof(GetTeamMeta)}";

            return await _cache.GetOrCreateAsync(cacheKey, GetTeamMetaInternal, CacheDuration.Medium);
        }

        private async Task<TeamMetaResult> GetTeamMetaInternal()
        {
            var projects = await _projectsApi.GetList();

            var teamsResultTasks = projects.Value
                .Select(async x => (
                    ProjectId: Convert.ToString(x.Id),
                    Project: x,
                    Teams: await _teamsApi.GetList(Convert.ToString(x.Id))))
                .ToList();

            await Task.WhenAll(teamsResultTasks);

            var teamsResult = teamsResultTasks
                .Select(x => x.Result)
                .SelectMany(x => x.Teams.Value.Select(t => (
                    ProjectId: x.ProjectId,
                    Project: x.Project,
                    TeamId: Convert.ToString(t.Id),
                    Team: t)))
                .ToList();

            var iterationsResultTasks = teamsResult.Select(async x => (
                    ProjectId: x.ProjectId,
                    Project: x.Project,
                    TeamId: x.TeamId,
                    Team: x.Team,
                    Iterations: await _iterationsApi.GetList(x.ProjectId, x.TeamId)))
                .ToList();

            await Task.WhenAll(iterationsResultTasks);

            var iterationsResult = iterationsResultTasks
                .Select(x => x.Result)
                .SelectMany(x => x.Iterations.Value.Select(
                    i => new IterationData
                    {
                        Project = x.Project,
                        Team = x.Team,
                        Iteration = i
                    }))
                .ToList();

            return await GetTeamMetaResult(iterationsResult);
        }

        private async Task<TeamMetaResult> GetTeamMetaResult(IEnumerable<IterationData> iterationData)
        {
            var projects = await GetTeamMetaProjects(iterationData);

            return new TeamMetaResult(projects);
        }

        private async Task<IList<TeamMetaProject>> GetTeamMetaProjects(
            IEnumerable<IterationData> iterationData)
        {
            var projectGroups = iterationData.GroupBy(x => Convert.ToString(x.Project.Id)).ToList();

            var teamMetaProjects = await GetTeamMetaProjects(projectGroups);

            return teamMetaProjects;
        }

        private async Task<List<TeamMetaProject>> GetTeamMetaProjects(
            IEnumerable<IGrouping<string, IterationData>> projectGroups)
        {
            var teamMetaProjects = new List<TeamMetaProject>();

            foreach (var projectGroup in projectGroups)
            {
                var projectId = projectGroup.Key;
                var project = projectGroup.Select(x => x.Project).First();

                var queriesTask = _queriesApi.GetList(projectId);
                var repositoriesTask = _gitApi.GetRepositoryList(projectId);

                var teamGroups = projectGroup.GroupBy(x => Convert.ToString(x.Team.Id)).ToList();

                var teamMetaTeamsTask = GetTeamMetaTeams(projectId, teamGroups);

                await Task.WhenAll(queriesTask, repositoriesTask, teamMetaTeamsTask);

                var queries = queriesTask.Result;
                var repositories = repositoriesTask.Result;
                var teamMetaTeams = teamMetaTeamsTask.Result;

                var teamMetaProject = new TeamMetaProject(project, queries, repositories, teamMetaTeams);
                teamMetaProjects.Add(teamMetaProject);
            }

            return teamMetaProjects;
        }

        private async Task<List<TeamMetaTeam>> GetTeamMetaTeams(
            string projectId,
            IEnumerable<IGrouping<string, IterationData>> teamGroups)
        {
            var teamMetaTeams = new List<TeamMetaTeam>();

            foreach (var teamGroup in teamGroups)
            {
                var teamId = teamGroup.Key;
                var team = teamGroup.Select(x => x.Team).First();

                var settingTask = _workApi.GetSetting(projectId, teamId);
                var teamMembersTask = _teamsApi.GetMembers(projectId, teamId);
                var boardsTask = _workApi.GetBoardList(projectId, teamId);

                var iterations = teamGroup
                    .Select(x => x.Iteration).ToList();

                var teamMetaIterationsTask = GetTeamMetaIterations(projectId, teamId, iterations);

                await Task.WhenAll(settingTask, teamMembersTask, boardsTask, teamMetaIterationsTask);

                var setting = settingTask.Result;
                var teamMembers = teamMembersTask.Result;
                var boards = boardsTask.Result;
                var teamMetaIterations = teamMetaIterationsTask.Result;

                var teamMetaMembers = teamMembers.Value.Select(x => new TeamMetaMember(x)).ToList();

                var teamMetaTeam = new TeamMetaTeam(team, setting, boards, teamMetaMembers, teamMetaIterations);
                teamMetaTeams.Add(teamMetaTeam);
            }

            return teamMetaTeams;
        }

        private async Task<List<TeamMetaIteration>> GetTeamMetaIterations(
            string projectId,
            string teamId,
            IEnumerable<IterationApiResponseBase> iterations)
        {
            var teamMetaIterations = new List<TeamMetaIteration>();

            foreach (var iteration in iterations)
            {
                var iterationId = Convert.ToString(iteration.Id);

                var capacitiesTask = _iterationsApi.GetCapacities(projectId, teamId, iterationId);
                var teamDaysOffTask = _iterationsApi.GetTeamDaysOff(projectId, teamId, iterationId);

                await Task.WhenAll(capacitiesTask, teamDaysOffTask);

                var capacities = capacitiesTask.Result;
                var teamDaysOff = teamDaysOffTask.Result;

                var teamMetaIteration = new TeamMetaIteration(iteration, capacities, teamDaysOff);
                teamMetaIterations.Add(teamMetaIteration);
            }

            return teamMetaIterations;
        }

        private class IterationData
        {
            public ProjectApiResponse Project { get; set; }

            public TeamApiResponse Team { get; set; }

            public IterationApiResponseBase Iteration { get; set; }
        }
    }
}