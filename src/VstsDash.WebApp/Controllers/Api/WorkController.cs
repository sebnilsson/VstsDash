using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VstsDash.AppServices.WorkActivity;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi;

namespace VstsDash.WebApp.Controllers.Api
{
    [Route("api/work", Name = RouteNames.ApiWork)]
    public class WorkController : ControllerBase
    {
        private readonly IIterationsApiService _iterationsApi;

        private readonly WorkActivityAppService _workActivityAppService;

        public WorkController(
            AppSettings appSettings,
            IIterationsApiService iterationsApi,
            IWorkApiService workApi,
            WorkActivityAppService workActivityAppService)
            : base(appSettings, workApi)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));
            if (workApi == null) throw new ArgumentNullException(nameof(workApi));

            _iterationsApi = iterationsApi ?? throw new ArgumentNullException(nameof(iterationsApi));
            _workActivityAppService = workActivityAppService ??
                                      throw new ArgumentNullException(nameof(workActivityAppService));
        }

        [HttpGet("teamactivities")]
        public async Task<IActionResult> TeamActivities(
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var workActivityData = await GetWorkActivityData(projectId, teamId, iterationId);

            var data = GetJsonData(workActivityData);

            return Json(data);
        }

        [HttpGet("memberactivities/{memberId}")]
        public async Task<IActionResult> MemberActivities(
            string memberId,
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            var workActivityData = await GetWorkActivityData(projectId, teamId, iterationId, memberId);

            var data = GetJsonData(workActivityData);

            return Json(data);
        }

        private static IList<object[]> GetJsonData(
            IEnumerable<(DateTime Date, List<CommitInfo> CommitInfos)> workActivityData)
        {
            return (from data in workActivityData
                orderby data.Date
                let isDateInPast = data.Date.Date <= DateTime.UtcNow.Date
                let commitCount = isDateInPast ? data.CommitInfos.Count : (int?) null
                let totalChangeCount = isDateInPast ? data.CommitInfos.Sum(c => c.Commit.TotalChangeCount) : (int?) null
                select new object[] {data.Date, commitCount, totalChangeCount}).ToList();
        }

        private async Task<IEnumerable<(DateTime Date, List<CommitInfo> CommitInfos)>> GetWorkActivityData(
            string projectId,
            string teamId,
            string iterationId,
            string memberId = null)
        {
            var idParams = await GetEnsuredIdParams(projectId, teamId, iterationId);

            var activity = await _workActivityAppService.GetActivity(
                idParams.ProjectId,
                idParams.TeamId,
                idParams.IterationId);

            IEnumerable<CommitInfo> commits = activity.Commits;
            if (!string.IsNullOrWhiteSpace(memberId))
            {
                var normalizedMemberId = NormalizeGuidId(memberId);

                commits = commits.Where(x => normalizedMemberId ==
                                             NormalizeGuidId(Convert.ToString(x.Author.MemberId)));
            }

            var iteration = await _iterationsApi.Get(idParams.ProjectId, idParams.TeamId, idParams.IterationId);
            var teamDaysOff =
                await _iterationsApi.GetTeamDaysOff(idParams.ProjectId, idParams.TeamId, idParams.IterationId);

            var capacity = new IterationCapacity(iteration, teamDaysOff);
            
            var dates = activity.FromDate.GetDatesUntil(activity.ToDate);

            return from date in dates
                let isWorkDay = capacity.NetWorkDays.Contains(date)
                let dayCommits = commits
                    .Where(x => (x.Commit.AuthorDate ?? DateTimeOffset.MinValue).Date == date.Date)
                    .ToList()
                where isWorkDay || dayCommits.Any()
                select (date, dayCommits);
        }

        private static string NormalizeGuidId(string id)
        {
            return id?.Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}