using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VstsDash.AppServices.WorkActivity
{
    public class AuthorCommits
    {
        public AuthorCommits(Author author, IEnumerable<CommitInfo> commits)
        {
            if (commits == null) throw new ArgumentNullException(nameof(commits));

            Author = author ?? throw new ArgumentNullException(nameof(author));
            Commits = new ReadOnlyCollection<CommitInfo>(commits.ToList());
        }

        public Author Author { get; }

        public int CommitCount => Commits.Count;

        public IReadOnlyCollection<CommitInfo> Commits { get; }

        public int CommitsTotalChangeCountSum => Commits.Sum(x => x.Commit.TotalChangeCount);
    }
}