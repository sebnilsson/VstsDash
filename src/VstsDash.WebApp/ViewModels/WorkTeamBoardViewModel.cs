using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.AppServices;

namespace VstsDash.WebApp.ViewModels
{
    public class WorkTeamBoardViewModel
    {
        public IReadOnlyCollection<(Player, Player.PlayerScore.Point)> AllPoints => GetAllPoints();

        public string IterationName { get; set; }

        public IReadOnlyCollection<Player> Players { get; set; }

        public IReadOnlyCollection<Player> PlayersWithCapacity => Players.Where(x => x.Capacity.TotalWorkDayCount > 0)
            .OrderBy(x => x.DisplayName)
            .ToList();

        public TeamBoardTeamCapacity TeamCapacity { get; set; }

        public double TotalHoursTotalCount { get; set; }

        public double TotalScoreAssistsSum { get; set; }

        public double TotalScoreGoalsSum { get; set; }

        public double TotalScorePointsSum { get; set; }

        public double TotalWorkDayCount { get; set; }

        public Player.PlayerScore UnassignedScore { get; set; }

        private IReadOnlyCollection<(Player Player, Player.PlayerScore.Point Point)> GetAllPoints()
        {
            return Players.SelectMany(player => player.Score.Points, (player, point) => (Player: player, Point: point))
                .OrderByDescending(x => x.Point.EarnedAt)
                .ToList();
        }

        public class Player
        {
            public PlayerCapacity Capacity { get; set; }

            public double CapacityMultiplier { get; set; }

            public string DisplayName { get; set; }

            public Guid Id { get; set; }

            public string ImageUrl { get; set; }

            public PlayerScore Score { get; set; }

            public double ScoreAssistsSum { get; set; }

            public double ScoreGoalsSum { get; set; }

            public double ScorePointsSum { get; set; }

            public string UniqueName { get; set; }

            public class PlayerCapacity
            {
                public double DailyHourCount { get; set; }

                public double DailyPercent { get; set; }

                public IReadOnlyCollection<DateTime> DaysOff { get; set; }

                public double HoursTotalCount { get; set; }

                public IReadOnlyCollection<DateTime> MemberDaysOff { get; set; }

                public double TotalWorkDayCount { get; set; }

                public IReadOnlyCollection<DateTime> WorkDays { get; set; }

                public double WorkDayPercent { get; set; }
            }

            public class PlayerScore
            {
                public IReadOnlyCollection<Point> Assists { get; set; }

                public IReadOnlyCollection<Point> Goals { get; set; }

                public IReadOnlyCollection<Point> Points { get; set; }

                public class Point
                {
                    public string Description { get; set; }

                    public DateTimeOffset EarnedAt { get; set; }

                    public bool HasBonus { get; set; }

                    public string Id { get; set; }

                    public TeamMemberPointType Type { get; set; }

                    public double Value { get; set; }
                }
            }
        }

        public class TeamBoardTeamCapacity
        {
            public double DailyHourCount { get; set; }

            public double DailyPercent { get; set; }

            public double HoursTotalCount { get; set; }

            public IReadOnlyCollection<DateTime> IterationWorkDays { get; set; }

            public IReadOnlyCollection<DateTime> TeamDaysOff { get; set; }

            public double TotalWorkDayCount { get; set; }

            public IReadOnlyCollection<DateTime> WorkDays { get; set; }
        }
    }
}