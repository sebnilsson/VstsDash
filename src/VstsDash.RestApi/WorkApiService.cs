using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;
using VstsDash.RestApi.Caching;

namespace VstsDash.RestApi
{
    public class WorkApiService : IWorkApiService, IApiService
    {
        private const int WorkItemListMaxCount = 200;

        private readonly IRestApiClient _apiClient;

        public WorkApiService(IRestApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<WorkBoardListApiResponse> GetBoardList(string projectId, string teamId)
        {
            var url = $"DefaultCollection/{projectId}/{teamId}/_apis/work/boards?api-version=3.0";

            return await _apiClient.Get<WorkBoardListApiResponse>(url, CacheDuration.Long);
        }

        public async Task<SettingApiResponse> GetSetting(string projectId, string teamId)
        {
            var url = $"/DefaultCollection/{projectId}/{teamId}/_apis/Work/TeamSettings?api-version=3.0";

            return await _apiClient.Get<SettingApiResponse>(url, CacheDuration.Medium);
        }

        public async Task<WorkItemListApiResponse> GetWorkItemList(params long[] workItemIds)
        {
            return await GetWorkItemList(null, null, workItemIds);
        }

        public async Task<WorkItemListApiResponse> GetWorkItemList(
            DateTime? asOf = null,
            IEnumerable<string> fields = null,
            params long[] workItemIds)
        {
            if (!workItemIds.Any())
            {
                return new WorkItemListApiResponse
                       {
                           Value = new List<WorkItemApiResponse>(0)
                       };
            }

            var fieldsList = fields?.ToList() ?? new List<string>(0);

            var pagedWorkItemIds = GetPagedWorkItemIds(workItemIds).ToList();

            var tasks = pagedWorkItemIds.Select(async x => await GetWorkItemListInternal(asOf, fieldsList, x)).ToList();

            await Task.WhenAll(tasks);

            var results = tasks.Select(x => x.Result).ToList();

            var model = new WorkItemListApiResponse();

            var values = results.SelectMany(x => x.Value).ToList();
            model.Value = new ReadOnlyCollection<WorkItemApiResponse>(values);

            return model;
        }

        private static IEnumerable<ICollection<long>> GetPagedWorkItemIds(ICollection<long> workItemIds)
        {
            ICollection<long> pagedWorkItemIds;

            var index = 0;
            do
            {
                var skipCount = index * WorkItemListMaxCount;

                pagedWorkItemIds = workItemIds.Skip(skipCount).Take(WorkItemListMaxCount).ToList();

                if (pagedWorkItemIds.Any()) yield return pagedWorkItemIds;

                index++;
            }
            while (pagedWorkItemIds.Any());
        }

        private async Task<WorkItemListApiResponse> GetWorkItemListInternal(
            DateTime? asOf,
            ICollection<string> fields,
            ICollection<long> workItemIds)
        {
            var idsQuery = workItemIds.Any() ? $"&ids={string.Join(",", workItemIds)}" : "&ids=";
            var asOfQuery = asOf != null ? $"&asOf={asOf:yyyy-MM-ddTHH:mm:ssZ}" : null;
            var fieldsQuery = fields.Any() ? $"&fields={string.Join(",", fields)}" : null;

            var url =
                $"DefaultCollection/_apis/wit/workitems?api-version=3.0{idsQuery}{asOfQuery}{fieldsQuery}&$expand=relations";

            return await _apiClient.Get<WorkItemListApiResponse>(url, CacheDuration.Short);
        }
    }
}