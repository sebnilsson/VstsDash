using System;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkActivity
{
    public class Repo
    {
        public Repo(GitRepositoryApiResponse repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            RepositoryId = repository.Id;
            RepositoryName = repository.Name;
        }

        public Guid RepositoryId { get; }

        public string RepositoryName { get; }
    }
}