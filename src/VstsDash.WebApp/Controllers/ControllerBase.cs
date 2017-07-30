using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VstsDash.RestApi;

namespace VstsDash.WebApp.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected readonly AppSettings AppSettings;

        protected readonly IWorkApiService WorkApi;

        protected ControllerBase(AppSettings appSettings, IWorkApiService workApi)
        {
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            WorkApi = workApi ?? throw new ArgumentNullException(nameof(workApi));
        }

        protected async Task<string> GetIterationIdOrDefault(string projectId, string teamId, string iterationId)
        {
            if (!string.IsNullOrWhiteSpace(iterationId))
                return iterationId;

            var setting = await WorkApi.GetSetting(projectId, teamId);

            var defaultIterationId = Convert.ToString(setting.DefaultIteration?.Id);

            if (string.IsNullOrWhiteSpace(defaultIterationId))
                throw new InvalidOperationException(
                    $"Could not resolve default Iteration ID for Project ID '{projectId}' and Team ID '{teamId}'.");

            return defaultIterationId;
        }

        protected string GetProjectIdOrDefault(string projectId)
        {
            var ensuredProjectId = !string.IsNullOrWhiteSpace(projectId)
                ? projectId
                : AppSettings.DefaultProjectId;
            return ensuredProjectId;
        }

        protected string GetRepositoryIdOrDefault(string repositoryId)
        {
            var ensuredRepositoryId = !string.IsNullOrWhiteSpace(repositoryId)
                ? repositoryId
                : AppSettings.DefaultRepositoryId;
            return ensuredRepositoryId;
        }

        protected string GetTeamIdOrDefault(string teamId)
        {
            var ensuredTeamId = !string.IsNullOrWhiteSpace(teamId) ? teamId : AppSettings.DefaultTeamId;
            return ensuredTeamId;
        }

        protected async Task<IdParams> GetEnsuredIdParams(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var ensuredProjectId = GetProjectIdOrDefault(projectId);
            var ensuredTeamId = GetTeamIdOrDefault(teamId);
            var ensuredIterationId = await GetIterationIdOrDefault(ensuredProjectId, ensuredTeamId, iterationId);

            return new IdParams(ensuredProjectId, ensuredTeamId, ensuredIterationId);
        }

        protected class IdParams
        {
            public IdParams(string projectId, string teamId, string iterationId)
            {
                ProjectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
                TeamId = teamId ?? throw new ArgumentNullException(nameof(teamId));
                IterationId = iterationId ?? throw new ArgumentNullException(nameof(iterationId));
            }

            public string IterationId { get; }

            public string ProjectId { get; }

            public string TeamId { get; }
        }
    }
}