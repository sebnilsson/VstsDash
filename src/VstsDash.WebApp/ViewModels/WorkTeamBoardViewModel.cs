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

        public double TeamWorkDaysCount => TeamCapacity.WorkDays.Count;

        public string TeamWorkDaysCountDisplay => TeamWorkDaysCount.ToString(Formats.NumberNoDecimals);

        public string TeamWorkDaysCountRemainingDisplay => GetTeamWorkDaysCountRemainingDisplay();

        public double TotalHoursTotalCount { get; set; }

        public string TotalHoursTotalCountDisplay => TotalHoursTotalCount.ToString(Formats.NumberNoDecimals);

        public string TotalHoursTotalCountRemainingDisplay => GetTotalHoursTotalCountRemainingDisplay();

        public double TotalScoreAssistsSum { get; set; }

        public string TotalScoreAssistsSumDisplay => GetPointDisplay(TotalScoreAssistsSum);

        public double TotalScoreGoalsSum { get; set; }

        public string TotalScoreGoalsSumDisplay => GetPointDisplay(TotalScoreGoalsSum);

        public double TotalScorePointsSum { get; set; }

        public string TotalScorePointsSumDisplay => GetPointDisplay(TotalScorePointsSum);

        public double TotalWorkDayCount { get; set; }

        public string TotalWorkDayCountDisplay => TotalWorkDayCount.ToString(Formats.NumberNoDecimals);

        public string TotalWorkDayCountRemainingDisplay => GetTotalWorkDayCountRemainingDisplay();

        public Player.PlayerScore UnassignedScore { get; set; }

        internal static string GetPointDisplay(double point)
        {
            return GetPositiveNumberDisplay(point);
        }

        internal static string GetPositiveNumberDisplay(double value, string suffix = null)
        {
            return value > 0 ? value.ToString(Formats.NumberOneDecimal) + suffix : "-";
        }

        private IReadOnlyCollection<(Player Player, Player.PlayerScore.Point Point)> GetAllPoints()
        {
            return Players.SelectMany(player => player.Score.Points, (player, point) => (Player: player, Point: point))
                          .OrderByDescending(x => x.Point.EarnedAt)
                          .ToList();
        }

        private string GetTeamWorkDaysCountRemainingDisplay()
        {
            var remaining = TeamCapacity.WorkDays.Count(x => x.Date >= DateTime.UtcNow.Date);

            bool shouldRemaingDisplay = remaining > 0 && remaining != TeamWorkDaysCount;
            return shouldRemaingDisplay ? remaining.ToString(Formats.NumberNoDecimals) + "/" : null;
        }

        private string GetTotalHoursTotalCountRemainingDisplay()
        {
            var remaining =
                Players.SelectMany(
                           player => player.Capacity.WorkDays,
                           (player, workDay) => (WorkDay: workDay, DailyHourCount: player.Capacity.DailyHourCount))
                       .Where(x => x.WorkDay.Date >= DateTime.UtcNow.Date)
                       .Sum(x => x.DailyHourCount);

            bool shouldRemainingDisplay = remaining > 0 && remaining != TotalHoursTotalCount;
            return shouldRemainingDisplay ? remaining.ToString(Formats.NumberNoDecimals) + "/" : null;
        }

        private string GetTotalWorkDayCountRemainingDisplay()
        {
            var remaining =
                Players.SelectMany(
                           player => player.Capacity.WorkDays,
                           (player, workDay) =>
                               (WorkDay: workDay, DailyPercentMultiplier: player.Capacity.DailyPercent / 100D))
                       .Where(x => x.WorkDay.Date >= DateTime.UtcNow.Date)
                       .Sum(x => x.DailyPercentMultiplier);

            bool shouldRemainingDisplay = remaining > 0 && remaining != TotalWorkDayCount;
            return shouldRemainingDisplay ? remaining.ToString(Formats.NumberNoDecimals) + "/" : null;
        }

        public class Player
        {
            public PlayerCapacity Capacity { get; set; }

            public string CapacityDailyHourCountDisplay => GetPositiveNumberDisplay(Capacity.DailyHourCount, "h");

            public double CapacityMultiplier { get; set; }

            public string DisplayName { get; set; }

            public Guid Id { get; set; }

            public string ImageUrl { get; set; }

            public PlayerScore Score { get; set; }

            public double ScoreAssistsSum { get; set; }

            public string ScoreAssistsSumDisplay => GetPointDisplay(ScoreAssistsSum);

            public double ScoreGoalsSum { get; set; }

            public string ScoreGoalsSumDisplay => GetPointDisplay(ScoreGoalsSum);

            public double ScorePointsSum { get; set; }

            public string ScorePointsSumDisplay => GetPointDisplay(ScorePointsSum);

            public string UniqueName { get; set; }

            public class PlayerCapacity
            {
                public double DailyHourCount { get; set; }

                public double DailyPercent { get; set; }

                public IReadOnlyCollection<DateTime> DaysOff { get; set; }

                public double HoursTotalCount { get; set; }

                public IReadOnlyCollection<DateTime> MemberDaysOff { get; set; }

                public double TotalWorkDayCount { get; set; }

                public double WorkDayPercent { get; set; }

                public IReadOnlyCollection<DateTime> WorkDays { get; set; }
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