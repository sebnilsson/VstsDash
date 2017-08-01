using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VstsDash.AppServices.WorkActivity;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.WebApp.Controllers.Api
{
    [Route("api/work", Name = RouteNames.ApiWork)]
    public class WorkController : ControllerBase
    {
        private readonly IIterationsApiService _iterationsApi;

        private readonly WorkActivityAppService _workActivityAppService;

        private readonly WorkIterationAppService _workIterationAppService;

        public WorkController(
            AppSettings appSettings,
            IIterationsApiService iterationsApi,
            IWorkApiService workApi,
            WorkActivityAppService workActivityAppService,
            WorkIterationAppService workIterationAppService)
            : base(appSettings, workApi)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));
            if (workApi == null) throw new ArgumentNullException(nameof(workApi));

            _iterationsApi = iterationsApi ?? throw new ArgumentNullException(nameof(iterationsApi));
            _workActivityAppService = workActivityAppService ??
                                      throw new ArgumentNullException(nameof(workActivityAppService));
            _workIterationAppService = workIterationAppService ??
                                       throw new ArgumentNullException(nameof(workIterationAppService));
        }

        [HttpGet("teamdoneefforts")]
        public async Task<IActionResult> TeamDoneEfforts(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var idParams = await GetEnsuredIdParams(projectId, teamId, iterationId);

            var efforts = await _workIterationAppService.GetWorkIterationDoneEffortsPerDay(
                idParams.ProjectId,
                idParams.TeamId,
                idParams.IterationId);

            var jsonData = efforts.Select(x => new object[] {x.Key, x.Value}).ToList();

            return Json(jsonData);
        }

        [HttpGet("teamactivities")]
        public async Task<IActionResult> TeamActivities(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var jsonData = await GetWorkActivityJsonData(projectId, teamId, iterationId);

            return Json(jsonData);
        }

        [HttpGet("memberactivities/{memberId}")]
        public async Task<IActionResult> MemberActivities(
            Guid memberId,
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var jsonData = await GetWorkActivityJsonData(projectId, teamId, iterationId, memberId);

            return Json(jsonData);
        }

        private async Task<IList<object[]>> GetWorkActivityJsonData(
            string projectId,
            string teamId,
            string iterationId,
            Guid? memberId = null)
        {
            var idParams = await GetEnsuredIdParams(projectId, teamId, iterationId);

            var activity = await _workActivityAppService.GetActivity(
                idParams.ProjectId,
                idParams.TeamId,
                idParams.IterationId);

            IEnumerable<CommitInfo> commits = activity.Commits;
            IterationCapacityListApiResponse iterationCapacities = null;
            if (memberId != null)
            {
                var normalizedMemberId = NormalizeGuidId(memberId.Value);

                commits = commits.Where(x => normalizedMemberId == NormalizeGuidId(x.Author.MemberId));

                iterationCapacities =
                    await _iterationsApi.GetCapacities(idParams.ProjectId, idParams.TeamId, idParams.IterationId);
            }

            var iteration = await _iterationsApi.Get(idParams.ProjectId, idParams.TeamId, idParams.IterationId);
            var teamDaysOff =
                await _iterationsApi.GetTeamDaysOff(idParams.ProjectId, idParams.TeamId, idParams.IterationId);

            var capacity = new IterationCapacity(iteration, teamDaysOff, iterationCapacities, memberId);

            var dates = activity.FromDate.GetWorkDatesUntil(activity.ToDate).OrderBy(x => x).ToList();

            return (from date in dates
                let dayCommits = commits
                    .Where(x => (x.Commit.AuthorDate ?? DateTimeOffset.MinValue).Date == date.Date)
                    .ToList()
                let isDateInPast = date.Date <= DateTime.UtcNow.Date
                let hasCommits = dayCommits.Any()
                let isWorkDay = capacity.NetWorkDays.Contains(date)
                let shouldIncludeData = hasCommits || isDateInPast && isWorkDay
                let commitCount = shouldIncludeData ? dayCommits.Count : (int?) null
                let totalChangeCount = shouldIncludeData
                    ? dayCommits.Sum(c => c.Commit.TotalChangeCount)
                    : (int?) null
                select new object[] {date, commitCount, totalChangeCount}).ToList();
        }

        private static string NormalizeGuidId(Guid id)
        {
            return Convert.ToString(id).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}