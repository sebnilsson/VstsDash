using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.TeamMeta
{
    public class TeamMetaTeam
    {
        public TeamMetaTeam(
            TeamApiResponse team,
            SettingApiResponse setting,
            WorkBoardListApiResponse boards,
            IEnumerable<TeamMetaMember> members,
            IEnumerable<TeamMetaIteration> iterations)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));
            if (setting == null) throw new ArgumentNullException(nameof(setting));
            if (boards == null) throw new ArgumentNullException(nameof(boards));
            if (members == null) throw new ArgumentNullException(nameof(members));
            if (iterations == null) throw new ArgumentNullException(nameof(iterations));

            Id = team.Id;
            Description = team.Description;
            Name = team.Name;
            Url = team.Url;

            BacklogIterationId = setting.BacklogIteration?.Id;
            BacklogIterationName = setting.BacklogIteration?.Name;
            DefaultIterationId = setting.DefaultIteration?.Id;
            DefaultIterationName = setting.DefaultIteration?.Name;

            var boardList = boards.Value.Select(x => new Board(x)).ToList();
            Boards = new ReadOnlyCollection<Board>(boardList);

            Members = new ReadOnlyCollection<TeamMetaMember>(members.ToList());
            Iterations = new ReadOnlyCollection<TeamMetaIteration>(iterations.ToList());
        }

        public Guid? BacklogIterationId { get; }

        public string BacklogIterationName { get; }

        public IReadOnlyCollection<Board> Boards { get; }

        public Guid? DefaultIterationId { get; }

        public string DefaultIterationName { get; }

        public string Description { get; }

        public Guid Id { get; }

        public IReadOnlyCollection<TeamMetaIteration> Iterations { get; }

        public IReadOnlyCollection<TeamMetaMember> Members { get; }

        public string Name { get; }

        public string Url { get; }

        public class Board
        {
            public Board(WorkBoardListApiResponse.Item board)
            {
                if (board == null) throw new ArgumentNullException(nameof(board));

                Id = board.Id;
                Name = board.Name;
                Url = board.Url;
            }

            public Guid Id { get; }

            public string Name { get; }

            public string Url { get; }
        }
    }
}