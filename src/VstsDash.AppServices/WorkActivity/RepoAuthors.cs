using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VstsDash.AppServices.WorkActivity
{
    public class RepoAuthors
    {
        public RepoAuthors(Repo repository, IEnumerable<AuthorCommits> authorCommits)
        {
            if (authorCommits == null) throw new ArgumentNullException(nameof(authorCommits));

            Repository = repository ?? throw new ArgumentNullException(nameof(repository));

            AuthorCommits = new ReadOnlyCollection<AuthorCommits>(authorCommits.ToList());
        }

        public IReadOnlyCollection<AuthorCommits> AuthorCommits { get; }

        public Repo Repository { get; }
    }
}