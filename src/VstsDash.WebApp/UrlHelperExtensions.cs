using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace VstsDash.WebApp
{
    public static class UrlHelperExtensions
    {
        public static string ApiFile(this IUrlHelper urlHelper, string url)
        {
            return urlHelper.RouteUrl(
                RouteNames.ApiFiles,
                new
                {
                    url
                });
        }

        public static string Empty(this IUrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.Empty);
        }

        public static string WorkActivity(
            this IUrlHelper urlHelper,
            bool inheritRouteValues)
        {
            var query = urlHelper.ActionContext.HttpContext.Request.Query;

            var projectId = Convert.ToString(query["projectId"]);
            var teamId = Convert.ToString(query["teamId"]);
            var iterationId = Convert.ToString(query["iterationId"]);
            
            return urlHelper.WorkActivity(projectId, teamId, iterationId, false);  
        }

            public static string WorkActivity(
            this IUrlHelper urlHelper,
            string projectId = null,
            string teamId = null,
            string iterationId = null,
            bool isDashboard = false)
        {
            return urlHelper.RouteUrl(
                RouteNames.Default,
                new
                {
                    Controller = "Work",
                    Action = "Activity",
                    projectId,
                    teamId,
                    iterationId,
                    dashboard = isDashboard ? "1" : null
                });
        }

        public static string WorkBacklog(this IUrlHelper urlHelper, string projectId = null, string teamId = null)
        {
            return urlHelper.RouteUrl(
                RouteNames.Default,
                new
                {
                    Controller = "Work",
                    Action = "Backlog",
                    projectId,
                    teamId
                });
        }

        public static string WorkIteration(
            this IUrlHelper urlHelper,
            string projectId = null,
            string teamId = null,
            string iterationId = null,
            DateTime? asOf = null)
        {
            var asOfValue = asOf != null ? $"{asOf:yyyy-MM-ddTHH:mm:ssZ}" : null;

            return urlHelper.RouteUrl(
                RouteNames.Default,
                new
                {
                    Controller = "Work",
                    Action = "Iteration",
                    projectId,
                    teamId,
                    iterationId,
                    asOf = asOfValue
                });
        }

        public static string WorkSprint(this IUrlHelper urlHelper, string projectId = null, string teamId = null)
        {
            return urlHelper.RouteUrl(
                RouteNames.Default,
                new
                {
                    Controller = "Work",
                    Action = "Sprint",
                    projectId,
                    teamId
                });
        }

        public static string WorkStories(
            this IUrlHelper urlHelper,
            bool inheritRouteValues)
        {
            var query = urlHelper.ActionContext.HttpContext.Request.Query;

            var projectId = Convert.ToString(query["projectId"]);
            var teamId = Convert.ToString(query["teamId"]);
            var iterationId = Convert.ToString(query["iterationId"]);

            return urlHelper.WorkStories(projectId, teamId, iterationId, false);
        }

        public static string WorkStories(
            this IUrlHelper urlHelper,
            string projectId = null,
            string teamId = null,
            string iterationId = null,
            bool isDashboard = false)
        {
            return urlHelper.RouteUrl(
                RouteNames.Default,
                new
                {
                    Controller = "Work",
                    Action = "Stories",
                    projectId,
                    teamId,
                    iterationId,
                    dashboard = isDashboard ? "1" : null
                });
        }

        public static string WorkTeamBoard(
            this IUrlHelper urlHelper,
            bool inheritRouteValues)
        {
            var query = urlHelper.ActionContext.HttpContext.Request.Query;

            var projectId = Convert.ToString(query["projectId"]);
            var teamId = Convert.ToString(query["teamId"]);
            var iterationId = Convert.ToString(query["iterationId"]);

            return urlHelper.WorkTeamBoard(projectId, teamId, iterationId, false);
        }

        public static string WorkTeamBoard(
            this IUrlHelper urlHelper,
            string projectId = null,
            string teamId = null,
            string iterationId = null,
            bool isDashboard = false)
        {
            return urlHelper.RouteUrl(
                RouteNames.Default,
                new
                {
                    Controller = "Work",
                    Action = "TeamBoard",
                    projectId,
                    teamId,
                    iterationId,
                    dashboard = isDashboard ? "1" : null
                });
        }

        public static string WorkTeamBoardMember(
            this IUrlHelper urlHelper,
            Guid memberId,
            string projectId = null,
            string teamId = null,
            string iterationId = null)
        {
            return urlHelper.RouteUrl(
                RouteNames.Default,
                new
                {
                    Controller = "Work",
                    Action = "TeamBoardMember",
                    Id = memberId,
                    projectId,
                    teamId,
                    iterationId
                });
        }
    }
}