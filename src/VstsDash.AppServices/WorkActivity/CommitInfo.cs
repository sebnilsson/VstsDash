using System;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkActivity
{
    public class CommitInfo
    {
        public CommitInfo(
            GitCommitApiResponse commit,
            TeamMemberApiResponse teamMember,
            GitRepositoryApiResponse repository)
        {
            if (commit == null) throw new ArgumentNullException(nameof(commit));
            if (teamMember == null) throw new ArgumentNullException(nameof(teamMember));
            if (repository == null) throw new ArgumentNullException(nameof(repository));

            Commit = new Commit(commit);
            Author = new Author(teamMember);
            Repository = new Repo(repository);
        }

        public Author Author { get; }

        public Commit Commit { get; }

        public Repo Repository { get; }
    }
}