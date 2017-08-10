using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VstsDash.RestApi;

namespace VstsDash.WebApp.Controllers.Api
{
    [Route("api/[controller]/[action]", Name = RouteNames.ApiMeta)]
    public class MetaController : ControllerBase
    {
        private readonly IProjectsApiService _projectsApi;

        private readonly IQueriesApiService _queriesApi;

        private readonly ITeamsApiService _teamsApi;

        public MetaController(
            AppSettings appSettings,
            IProjectsApiService projectsApi,
            IQueriesApiService queriesApi,
            ITeamsApiService teamsApi,
            IWorkApiService workApi)
            : base(appSettings, workApi)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));
            if (workApi == null) throw new ArgumentNullException(nameof(workApi));

            _projectsApi = projectsApi ?? throw new ArgumentNullException(nameof(projectsApi));
            _queriesApi = queriesApi ?? throw new ArgumentNullException(nameof(queriesApi));
            _teamsApi = teamsApi ?? throw new ArgumentNullException(nameof(teamsApi));
        }

        public async Task<IActionResult> Projects()
        {
            var projects = await _projectsApi.GetList();

            return Json(
                projects,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> Queries(string projectId)
        {
            var projects = await _queriesApi.GetList(projectId);

            return Json(
                projects,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> Teams(string projectId)
        {
            var teams = await _teamsApi.GetList(projectId);

            return Json(
                teams,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
        }
    }
}