using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VstsDash.AppServices.WorkActivity;
using VstsDash.AppServices.WorkIteration;
using VstsDash.AppServices.WorkTeamBoard;
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

        private readonly WorkTeamBoardAppService _workTeamBoardAppService;

        public WorkController(
            AppSettings appSettings,
            IMapper mapper,
            IWorkApiService workApi,
            WorkIterationAppService workIterationAppService,
            WorkActivityAppService workActivityAppService,
            WorkTeamBoardAppService workTeamBoardAppService)
            : base(appSettings, workApi)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _workApi = workApi ?? throw new ArgumentNullException(nameof(workApi));
            _workActivityAppService = workActivityAppService
                                      ?? throw new ArgumentNullException(nameof(workActivityAppService));
            _workIterationAppService = workIterationAppService
                                       ?? throw new ArgumentNullException(nameof(workIterationAppService));
            _workTeamBoardAppService = workTeamBoardAppService
                                       ?? throw new ArgumentNullException(nameof(workTeamBoardAppService));
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

        public async Task<IActionResult> Sprint(string projectId = null, string teamId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);

            var setting = await _workApi.GetSetting(ensuredProjectId, ensuredTeamId);
            var defaultIterationId = Convert.ToString(setting.DefaultIteration?.Id);

            var redirectUrl = Url.WorkIteration(ensuredProjectId, ensuredTeamId, defaultIterationId);
            return Redirect(redirectUrl);
        }

        public async Task<IActionResult> TeamBoard(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);
            var ensuredIterationId = await GetIterationIdOrDefault(ensuredProjectId, ensuredTeamId, iterationId);

            var teamBoard =
                await _workTeamBoardAppService.GetTeamBoard(ensuredProjectId, ensuredTeamId, ensuredIterationId);

            var model = _mapper.Map<WorkTeamBoardViewModel>(teamBoard);
            return View(model);
        }

        public async Task<IActionResult> TeamBoardMember(
            Guid id,
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);
            var ensuredIterationId = await GetIterationIdOrDefault(ensuredProjectId, ensuredTeamId, iterationId);

            var teamBoard =
                await _workTeamBoardAppService.GetTeamBoard(ensuredProjectId, ensuredTeamId, ensuredIterationId);

            var teamBoardModel = _mapper.Map<WorkTeamBoardViewModel>(teamBoard);

            var model = teamBoardModel.Players.FirstOrDefault(x => x.Id == id);
            return View(model);
        }
    }
}