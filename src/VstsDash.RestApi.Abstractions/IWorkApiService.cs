using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    public interface IWorkApiService
    {
        Task<WorkBoardListApiResponse> GetBoardList(string projectId, string teamId);

        Task<SettingApiResponse> GetSetting(string projectId, string teamId);

        Task<WorkItemListApiResponse> GetWorkItemList(params long[] workItemIds);

        Task<WorkItemListApiResponse> GetWorkItemList(
            DateTime? asOf = null,
            IEnumerable<string> fields = null,
            params long[] workItemIds);
    }
}