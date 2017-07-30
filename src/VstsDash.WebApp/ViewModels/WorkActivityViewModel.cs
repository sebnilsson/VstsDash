using System;
using System.Collections.Generic;
using System.Linq;

namespace VstsDash.WebApp.ViewModels
{
    public class WorkActivityViewModel
    {
        public IReadOnlyCollection<AuthorCommits> Authors { get; set; }

        public int AuthorsCommitsTotalChangeCountSum => Authors.Sum(ac => ac.CommitsTotalChangeCountSum);

        public int AuthorsCommitCountSum => Authors.Sum(ac => ac.CommitCount);

        public IReadOnlyCollection<CommitInfo> Commits { get; set; }

        public DateTime FromDate { get; set; }

        public string IterationName { get; set; }

        public IReadOnlyCollection<RepositoryAuthors> Repos { get; set; }

        public DateTime ToDate { get; set; }

        public class Author
        {
            public Guid MemberId { get; set; }

            public string MemberDisplayName { get; set; }

            public string MemberImageUrl { get; set; }

            public string MemberUniqueName { get; set; }
        }

        public class AuthorCommits
        {
            public Author Author { get; set; }

            public int CommitCount => Commits.Count;

            public IReadOnlyCollection<CommitInfo> Commits { get; set; }

            public int CommitsTotalChangeCountSum => Commits.Sum(x => x.Commit.TotalChangeCount);

            public int MaxCommitDayCount => KnownAuthorCommits.Any()
                ? KnownAuthorCommits
                    .GroupBy(x => (x.Commit.AuthorDate ?? DateTime.MinValue).Date)
                    .Max(g => g.Count())
                : 0;

            public int MaxChangeDayCount => KnownAuthorCommits.Any()
                ? KnownAuthorCommits
                    .GroupBy(x => (x.Commit.AuthorDate ?? DateTime.MinValue).Date)
                    .Max(g => g.Sum(x => x.Commit.TotalChangeCount))
                : 0;

            private IEnumerable<CommitInfo> KnownAuthorCommits => Commits.Where(x => x.Author.MemberId != Guid.Empty);
        }

        public class Commit
        {
            public int AddChangeCount { get; set; }

            public DateTimeOffset? AuthorDate { get; set; }

            public string AuthorEmail { get; set; }

            public string AuthorName { get; set; }

            public string Comment { get; set; }

            public string CommitId { get; set; }

            public DateTimeOffset? CommitterDate { get; set; }

            public string CommitterEmail { get; set; }

            public string CommitterName { get; set; }

            public int DeleteChangeCount { get; set; }

            public int EditChangeCount { get; set; }

            public int TotalChangeCount { get; set; }
        }

        public class CommitInfo
        {
            public Author Author { get; set; }

            public Commit Commit { get; set; }

            public Repository Repository { get; set; }
        }

        public class Repository
        {
            public Guid RepositoryId { get; set; }

            public string RepositoryName { get; set; }
        }

        public class RepositoryAuthors
        {
            public IReadOnlyCollection<AuthorCommits> AuthorCommits { get; set; }

            public Repository Repository { get; set; }

            public int AuthorCommitsCountSum => AuthorCommits.Sum(x => x.CommitCount);
        }
    }
}