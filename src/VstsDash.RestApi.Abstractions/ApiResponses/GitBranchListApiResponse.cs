using System.Collections.Generic;
using System.Linq;

namespace VstsDash.RestApi.ApiResponses
{
    public class GitBranchListApiResponse : ListApiResponseBase<GitBranchApiResponse>
    {
        public IReadOnlyCollection<string> BranchNames => Value
            .Select(x =>
            {
                var lastSlashIndex = x.Name?.LastIndexOf('/') ?? -1;
                return lastSlashIndex >= 0 ? x.Name?.Substring(lastSlashIndex + 1) : null;
            })
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
}