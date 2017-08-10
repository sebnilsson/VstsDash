using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class GitRepositoryApiResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public GitRepositoriesListResponseProject Project { get; set; }

        public string RemoteUrl { get; set; }

        public string Url { get; set; }

        public class GitRepositoriesListResponseProject
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public string State { get; set; }

            public string Url { get; set; }
        }
    }
}