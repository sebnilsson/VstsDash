using System;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkActivity
{
    public class Commit
    {
        public Commit(GitCommitApiResponse gitCommit)
        {
            if (gitCommit == null) throw new ArgumentNullException(nameof(gitCommit));

            Comment = gitCommit.Comment;
            CommitId = gitCommit.CommitId;

            AuthorDate = gitCommit.Author?.Date;
            AuthorEmail = gitCommit.Author?.Email;
            AuthorName = gitCommit.Author?.Name;

            CommitterDate = gitCommit.Committer?.Date;
            CommitterEmail = gitCommit.Committer?.Email;
            CommitterName = gitCommit.Committer?.Name;

            AddChangeCount = gitCommit.ChangeCounts?.Add ?? 0;
            DeleteChangeCount = gitCommit.ChangeCounts?.Delete ?? 0;
            EditChangeCount = gitCommit.ChangeCounts?.Edit ?? 0;
        }

        public int AddChangeCount { get; }

        public DateTimeOffset? AuthorDate { get; }

        public string AuthorEmail { get; }

        public string AuthorName { get; }

        public string Comment { get; }

        public string CommitId { get; }

        public DateTimeOffset? CommitterDate { get; }

        public string CommitterEmail { get; }

        public string CommitterName { get; }

        public int DeleteChangeCount { get; }

        public int EditChangeCount { get; }

        public int TotalChangeCount => AddChangeCount + DeleteChangeCount + EditChangeCount;
    }
}