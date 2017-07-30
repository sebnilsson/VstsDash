using System.Threading.Tasks;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.RestApi
{
    public interface IWiqlApiService
    {
        Task<WiqlWorkItemLinkApiResponse> QueryWorkItems(
            string projectId,
            string iterationPath,
            bool isBacklog = false);
    }
}