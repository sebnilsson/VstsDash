using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.TeamMeta
{
    public class TeamMetaProject
    {
        public TeamMetaProject(
            ProjectApiResponse project,
            QueryListApiResponse queries,
            GitRepositoryListApiResponse repositories,
            IEnumerable<TeamMetaTeam> teams)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            if (queries == null) throw new ArgumentNullException(nameof(queries));
            if (repositories == null) throw new ArgumentNullException(nameof(repositories));
            if (teams == null) throw new ArgumentNullException(nameof(teams));

            Id = project.Id;
            Description = project.Description;
            Name = project.Name;
            State = project.State;
            Url = project.Url;

            var queryList = queries.Value.Select(x => new Query(x)).ToList();
            var repositoryList = repositories.Value.Select(x => new Repository(x)).ToList();

            Queries = new ReadOnlyCollection<Query>(queryList);
            Repositories = new ReadOnlyCollection<Repository>(repositoryList);

            Teams = new ReadOnlyCollection<TeamMetaTeam>(teams.ToList());
        }

        public string Description { get; }

        public Guid Id { get; }

        public string Name { get; }

        public IReadOnlyCollection<Query> Queries { get; }

        public IReadOnlyCollection<Repository> Repositories { get; }

        public string State { get; }

        public IReadOnlyCollection<TeamMetaTeam> Teams { get; }

        public string Url { get; }

        public class Query
        {
            public Query(QueryApiResponse query)
            {
                if (query == null) throw new ArgumentNullException(nameof(query));

                Id = query.Id;
                CreatedDate = query.CreatedDate;
                HasChildren = query.HasChildren;
                IsFolder = query.IsFolder;
                IsPublic = query.IsPublic;
                LastModifiedDate = query.LastModifiedDate;
                Name = query.Name;
                Path = query.Path;
                QueryType = query.QueryType;
                Url = query.Url;
                Wiql = query.Wiql;

                var children = query.Children.Select(x => new Query(x)).ToList();
                Children = new ReadOnlyCollection<Query>(children);
            }

            public IReadOnlyCollection<Query> Children { get; }

            public DateTimeOffset CreatedDate { get; }

            public bool HasChildren { get; }

            public Guid Id { get; }

            public bool IsFolder { get; }

            public bool IsPublic { get; }

            public DateTimeOffset LastModifiedDate { get; }

            public string Name { get; }

            public string Path { get; }

            public string QueryType { get; }

            public string Url { get; }

            public string Wiql { get; }
        }

        public class Repository
        {
            public Repository(GitRepositoryApiResponse repository)
            {
                if (repository == null) throw new ArgumentNullException(nameof(repository));

                Id = repository.Id;
                Name = repository.Name;
                RemoteUrl = repository.RemoteUrl;
                Url = repository.Url;
            }

            public Guid Id { get; }

            public string Name { get; }

            public string RemoteUrl { get; }

            public string Url { get; }
        }
    }
}