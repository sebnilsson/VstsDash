using System.Collections.Generic;
using System.Linq;

namespace VstsDash.RestApi.ApiResponses
{
    public class GitBranchListApiResponse : ListApiResponseBase<GitBranchApiResponse>
    {
        public IReadOnlyCollection<string> BranchNames => Value
            .Select(x =>
            {
                var name = x?.Name;
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                var firstSlashIndex = name.IndexOf('/');
                var secondSlashIndex = firstSlashIndex >= 0 ? name.IndexOf('/', firstSlashIndex + 1) : -1;

                return secondSlashIndex >= 0 ? name.Substring(secondSlashIndex + 1) : null;
            })
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
}