using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class GitCommitApiResponse
    {
        public string CommitId { get; set; }

        public GitCommitsListResponseAuthor Author { get; set; }

        public GitCommitsListResponseCommitter Committer { get; set; }

        public string Comment { get; set; }

        public GitCommitsListResponseChangeCounts ChangeCounts { get; set; }

        public string Url { get; set; }

        public string RemoteUrl { get; set; }

        public class GitCommitsListResponseAuthor
        {
            public string Name { get; set; }

            public string Email { get; set; }

            public DateTimeOffset Date { get; set; }
        }

        public class GitCommitsListResponseCommitter
        {
            public string Name { get; set; }

            public string Email { get; set; }

            public DateTimeOffset Date { get; set; }
        }

        public class GitCommitsListResponseChangeCounts
        {
            public int Add { get; set; }

            public int Delete { get; set; }

            public int Edit { get; set; }
        }
    }
}