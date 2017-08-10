using System;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    // https://blogs.msdn.microsoft.com/team_foundation/2010/07/02/wiql-syntax-for-link-query/

    public class WiqlApiService : IWiqlApiService, IApiService
    {
        private readonly IRestApiClient _apiClient;

        public WiqlApiService(IRestApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<WiqlWorkItemLinkApiResponse> QueryWorkItems(
            string projectId,
            string iterationPath,
            bool isBacklog = false)
        {
            var url = $"DefaultCollection/{projectId}/_apis/wit/wiql?api-version=3.0";

            var query = isBacklog ? GetBacklogQuery(iterationPath) : GetIterationQuery(iterationPath);
            var json = new
                       {
                           query
                       };

            return await _apiClient.Post<WiqlWorkItemLinkApiResponse>(url, json);
        }

        private static string GetBacklogQuery(string iterationPath)
        {
            var query =
                "SELECT [System.WorkItemType], [System.Title], [System.State], [Microsoft.VSTS.Scheduling.Effort], [System.AreaPath], [System.IterationPath], [System.Tags], [System.Description] "
                + "FROM WorkItemLinks " + "WHERE (Source.[System.WorkItemType] in ('Product Backlog Item', 'Bug')) and "
                + $"Source.[System.IterationPath] under '{iterationPath}' and "
                + $"Source.[System.State] in ('New', 'Approved', 'Committed') and "
                + $"[System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward' and "
                + $"((Target.[System.WorkItemType] in ('Product Backlog Item', 'Bug')) and "
                + $"(Target.[System.Id] > 0) and " + $"Target.[System.IterationPath] under '{iterationPath}' "
                + $"and Target.[System.State] in ('New', 'Approved', 'Committed') or "
                + $"((Target.[System.WorkItemType] in ('Task')) and "
                + $"Target.[System.State] in ('To Do', 'In Progress', 'Done'))) "
                + $"ORDER BY [Microsoft.VSTS.Common.BacklogPriority] ASC, [System.Id] ASC MODE (Recursive)";
            return query;
        }

        private static string GetIterationQuery(string iterationPath)
        {
            var iterationRootIndex = iterationPath.IndexOf('\\');
            var iterationRoot = iterationRootIndex >= 0
                                    ? iterationPath.Substring(0, iterationRootIndex)
                                    : iterationPath;

            var query =
                "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], [Microsoft.VSTS.Scheduling.Effort], [Microsoft.VSTS.Scheduling.RemainingWork], [System.Tags], [System.AssignedTo], [Microsoft.VSTS.Common.BacklogPriority], [System.AreaId], [System.AreaPath], [Microsoft.VSTS.Common.Activity] "
                + "FROM WorkItemLinks " + "WHERE ((Source.[System.WorkItemType] in ('Product Backlog Item', 'Bug') and "
                + "Source.[System.State] in ('New', 'Approved', 'Committed', 'Done')) and "
                + $"Source.[System.IterationPath] under '{iterationRoot}') and "
                + "([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') and "
                + "(((Target.[System.WorkItemType] in ('Task') and "
                + "Target.[System.State] in ('To Do', 'In Progress', 'Done')) or "
                + "(Target.[System.WorkItemType] in ('Product Backlog Item', 'Bug') and "
                + "Target.[System.State] in ('New', 'Approved', 'Committed', 'Done'))) and "
                + $"Target.[System.IterationPath] under '{iterationPath}') "
                + "ORDER BY [Microsoft.VSTS.Common.BacklogPriority] ASC, [System.Id] ASC MODE (Recursive, ReturnMatchingChildren)";
            return query;
        }
    }
}