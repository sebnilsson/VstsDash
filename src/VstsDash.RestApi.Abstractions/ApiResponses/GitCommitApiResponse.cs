using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class GitCommitApiResponse
    {
        public GitCommitsListResponseAuthor Author { get; set; }

        public GitCommitsListResponseChangeCounts ChangeCounts { get; set; }

        public string Comment { get; set; }

        public string CommitId { get; set; }

        public GitCommitsListResponseCommitter Committer { get; set; }

        public string RemoteUrl { get; set; }

        public string Url { get; set; }

        public class GitCommitsListResponseAuthor
        {
            public DateTimeOffset Date { get; set; }

            public string Email { get; set; }

            public string Name { get; set; }
        }

        public class GitCommitsListResponseChangeCounts
        {
            public int Add { get; set; }

            public int Delete { get; set; }

            public int Edit { get; set; }
        }

        public class GitCommitsListResponseCommitter
        {
            public DateTimeOffset Date { get; set; }

            public string Email { get; set; }

            public string Name { get; set; }
        }
    }
}