using System;
using System.Collections.Generic;

namespace VstsDash.RestApi.ApiResponses
{
    public class GitFullCommitApiResponse : GitCommitApiResponse
    {
        public IReadOnlyCollection<string> Parents { get; set; }

        public CommitPush Push { get; set; }

        public string TreeId { get; set; }

        public class CommitPush
        {
            public DateTimeOffset Date { get; set; }

            public CommitPushedBy PushedBy { get; set; }

            public int PushId { get; set; }

            public class CommitPushedBy
            {
                public string DisplayName { get; set; }

                public string Id { get; set; }

                public string ImageUrl { get; set; }

                public string UniqueName { get; set; }
            }
        }
    }
}