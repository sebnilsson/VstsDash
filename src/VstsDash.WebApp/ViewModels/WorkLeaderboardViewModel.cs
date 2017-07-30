using System;
using System.Collections.Generic;
using VstsDash.AppServices;

namespace VstsDash.WebApp.ViewModels
{
    public class WorkLeaderboardViewModel
    {
        public Player.PlayerCapacity IterationCapacity { get; set; }

        public string IterationName { get; set; }

        public IReadOnlyCollection<Player> Players { get; set; }

        public double TotalHoursTotalCount { get; set; }

        public double TotalScoreAssistsSum { get; set; }

        public double TotalScoreGoalsSum { get; set; }

        public double TotalScorePointsSum { get; set; }

        public double TotalWorkDaysTotalCount { get; set; }

        public Player.PlayerScore UnassignedScore { get; set; }

        public class Player
        {
            public PlayerCapacity Capacity { get; set; }

            public string DisplayName { get; set; }

            public Guid Id { get; set; }

            public string ImageUrl { get; set; }

            public PlayerScore Score { get; set; }

            public double ScoreAssistsSum { get; set; }

            public double ScorePointsSum { get; set; }

            public double ScoreGoalsSum { get; set; }

            public string UniqueName { get; set; }

            public class PlayerCapacity
            {
                public double DailyHourCount { get; set; }

                public double DailyPercent { get; set; }

                public double HoursTotalCount { get; set; }

                public double Multiplier { get; set; }

                public IReadOnlyCollection<DateTime> AllDaysOff { get; set; }

                public IReadOnlyCollection<DateTime> IterationWorkDays { get; set; }

                public IReadOnlyCollection<DateTime> NetWorkDays { get; set; }

                public IReadOnlyCollection<DateTime> MemberDaysOff { get; set; }

                public IReadOnlyCollection<DateTime> TeamDaysOff { get; set; }

                public double WorkDaysTotalCount { get; set; }
            }

            public class PlayerScore
            {
                public IReadOnlyCollection<Point> Assists { get; set; }

                public IReadOnlyCollection<Point> Goals { get; set; }

                public IReadOnlyCollection<Point> Points { get; set; }

                public class Point
                {
                    public DateTimeOffset EarnedAt { get; set; }

                    public string Description { get; set; }

                    public string Id { get; set; }

                    public TeamMemberPointType Type { get; set; }

                    public double Value { get; set; }
                }
            }
        }
    }
}