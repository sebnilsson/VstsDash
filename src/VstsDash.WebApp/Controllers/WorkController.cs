using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VstsDash.AppServices.WorkActivity;
using VstsDash.AppServices.WorkIteration;
using VstsDash.AppServices.WorkLeaderboard;
using VstsDash.RestApi;
using VstsDash.WebApp.ViewModels;

namespace VstsDash.WebApp.Controllers
{
    public class WorkController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly WorkActivityAppService _workActivityAppService;

        private readonly IWorkApiService _workApi;

        private readonly WorkIterationAppService _workIterationAppService;

        private readonly WorkLeaderboardAppService _workLeaderboardAppService;

        public WorkController(
            AppSettings appSettings,
            IMapper mapper,
            IWorkApiService workApi,
            WorkIterationAppService workIterationAppService,
            WorkActivityAppService workActivityAppService,
            WorkLeaderboardAppService workLeaderboardAppService)
            : base(appSettings, workApi)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _workApi = workApi ?? throw new ArgumentNullException(nameof(workApi));
            _workActivityAppService = workActivityAppService
                                      ?? throw new ArgumentNullException(nameof(workActivityAppService));
            _workIterationAppService = workIterationAppService
                                       ?? throw new ArgumentNullException(nameof(workIterationAppService));
            _workLeaderboardAppService = workLeaderboardAppService
                                         ?? throw new ArgumentNullException(nameof(workLeaderboardAppService));
        }

        public async Task<IActionResult> Activity(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var idParams = await GetEnsuredIdParams(projectId, teamId, iterationId);

            var workActivity =
                await _workActivityAppService.GetActivity(idParams.ProjectId, idParams.TeamId, idParams.IterationId);

            var model = _mapper.Map<WorkActivityViewModel>(workActivity);

            var test = model.Authors
                .Select(
                    r => (Item: r, Score: r.CommitCount / (double)model.AuthorsCommitCountSum
                                          + r.CommitsTotalChangeCountSum
                                          / (double)model.AuthorsCommitsTotalChangeCountSum))
                .OrderByDescending(x => x.Score)
                .ToList();

            return View(model);
        }

        public async Task<IActionResult> Backlog(string projectId = null, string teamId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);

            var setting = await _workApi.GetSetting(ensuredProjectId, ensuredTeamId);
            var backlogIterationId = Convert.ToString(setting.BacklogIteration?.Id);

            var redirectUrl = Url.WorkIteration(ensuredProjectId, ensuredTeamId, backlogIterationId);
            return Redirect(redirectUrl);
        }

        public async Task<IActionResult> Iteration(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);
            var ensuredIterationId = await GetIterationIdOrDefault(ensuredProjectId, ensuredTeamId, iterationId);

            var model = new WorkIterationViewModel(ensuredProjectId, ensuredTeamId, ensuredIterationId);
            await model.Update(_workIterationAppService);

            return View(model);
        }

        public async Task<IActionResult> Leaderboard(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);
            var ensuredIterationId = await GetIterationIdOrDefault(ensuredProjectId, ensuredTeamId, iterationId);

            var leaderboard =
                await _workLeaderboardAppService.GetLeaderboard(ensuredProjectId, ensuredTeamId, ensuredIterationId);

            var model = _mapper.Map<WorkLeaderboardViewModel>(leaderboard);
            return View(model);
        }

        public async Task<IActionResult> Sprint(string projectId = null, string teamId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);

            var setting = await _workApi.GetSetting(ensuredProjectId, ensuredTeamId);
            var defaultIterationId = Convert.ToString(setting.DefaultIteration?.Id);

            var redirectUrl = Url.WorkIteration(ensuredProjectId, ensuredTeamId, defaultIterationId);
            return Redirect(redirectUrl);
        }
    }
}