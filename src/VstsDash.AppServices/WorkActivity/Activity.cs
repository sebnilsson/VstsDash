using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkActivity
{
    public class Activity
    {
        public Activity(
            IEnumerable<CommitInfo> commitData,
            DateTime fromDate,
            DateTime toDate,
            TeamCapacity teamCapacity,
            IReadOnlyDictionary<DateTime, double?> effortDone,
            IterationApiResponse iteration = null)
        {
            if (commitData == null) throw new ArgumentNullException(nameof(commitData));

            TeamCapacity = teamCapacity ?? throw new ArgumentNullException(nameof(teamCapacity));
            EffortDone = effortDone ?? throw new ArgumentNullException(nameof(effortDone));

            FromDate = fromDate;
            ToDate = toDate;

            IterationName = iteration?.Name;

            var commits = commitData.ToList();

            Commits = new ReadOnlyCollection<CommitInfo>(commits);

            var authors = GetAuthors(commits).ToList();
            Authors = new ReadOnlyCollection<AuthorCommits>(authors);

            var repos = GetRepos(commits).ToList();
            Repos = new ReadOnlyCollection<RepoAuthors>(repos);
        }

        public IReadOnlyCollection<AuthorCommits> Authors { get; }

        public int AuthorsCommitsTotalChangeCountSum => Authors.Sum(ac => ac.CommitsTotalChangeCountSum);

        public int AuthorsCommitCountSum => Authors.Sum(ac => ac.CommitCount);

        public IReadOnlyCollection<CommitInfo> Commits { get; }

        public IReadOnlyDictionary<DateTime, double?> EffortDone { get; }

        public DateTime FromDate { get; }

        public TeamCapacity TeamCapacity { get; }

        public string IterationName { get; }

        public IReadOnlyCollection<RepoAuthors> Repos { get; }

        public DateTime ToDate { get; }

        private static IEnumerable<AuthorCommits> GetAuthors(IReadOnlyCollection<CommitInfo> commitInfo)
        {
            var authors = commitInfo.Select(x => x.Author).Distinct(x => x.MemberId).ToList();

            foreach (var author in authors)
            {
                var commits = commitInfo
                    .Where(x => x.Author.MemberId == author.MemberId)
                    .Distinct(x => x.Commit.CommitId)
                    .ToList();

                yield return new AuthorCommits(author, commits);
            }
        }

        private static IEnumerable<RepoAuthors> GetRepos(IReadOnlyCollection<CommitInfo> commitInfo)
        {
            var repositories = commitInfo.Select(x => x.Repository).Distinct(x => x.RepositoryId).ToList();

            foreach (var repository in repositories)
            {
                var authors = commitInfo
                    .Where(x => x.Repository.RepositoryId == repository.RepositoryId)
                    .Select(x => x.Author)
                    .Distinct(x => x.MemberId)
                    .ToList();

                var authorCommits = (from author in authors
                    let commits = commitInfo
                        .Where(x => x.Repository.RepositoryId == repository.RepositoryId
                                    && x.Author.MemberId == author.MemberId)
                        .Distinct(x => x.Commit.CommitId)
                        .ToList()
                    select new AuthorCommits(author, commits)).ToList();

                yield return new RepoAuthors(repository, authorCommits);
            }
        }
    }
}