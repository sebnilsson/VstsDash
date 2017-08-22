using System;
using Microsoft.AspNetCore.Mvc;

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