namespace VstsDash.WebApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using VstsDash.AppServices;

    public class WorkTeamBoardViewModel
    {
        public IReadOnlyCollection<(Player, Player.PlayerScore.Point)> AllPoints => GetAllPoints();

        public string IterationName { get; set; }

        public IReadOnlyCollection<Player> Players { get; set; }

        public IReadOnlyCollection<Player> PlayersWithCapacity => Players.Where(x => x.Capacity.TotalWorkDayCount > 0)
            .OrderBy(x => x.DisplayName)
            .ToList();

        public TeamBoardTeamCapacity TeamCapacity { get; set; }

        public string TeamWorkDaysCountDisplay => GetTeamWorkDaysCountDisplay();

        public double TotalHoursTotalCount { get; set; }

        public string TotalHoursTotalCountDisplay => GetTotalHoursTotalCountDisplay();

        public double TotalScoreAssistsSum { get; set; }

        public string TotalScoreAssistsSumDisplay => GetPointDisplay(TotalScoreAssistsSum);

        public double TotalScoreGoalsSum { get; set; }

        public string TotalScoreGoalsSumDisplay => GetPointDisplay(TotalScoreGoalsSum);

        public double TotalScorePointsSum { get; set; }

        public string TotalScorePointsSumDisplay => GetPointDisplay(TotalScorePointsSum);

        public double TotalWorkDayCount { get; set; }

        public string TotalWorkDayCountDisplay => GetTotalWorkDayCountDisplay();

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

        private string GetTeamWorkDaysCountDisplay()
        {
            var workDays = TeamCapacity.WorkDays.ToList();

            var workDaysCount = workDays.Count;
            var workDaysCountDisplay = workDaysCount.ToString(Formats.NumberNoDecimals);

            var remainingCount = workDays.Count(x => x.Date >= DateTime.UtcNow.Date);
            if (remainingCount <= 0 || remainingCount == workDaysCount)
            {
                return $"{workDaysCountDisplay}d";
            }

            var remainingCountDisplay = remainingCount.ToString(Formats.NumberNoDecimals);

            return $"{remainingCountDisplay}/{workDaysCountDisplay}d";
        }

        private string GetTotalHoursTotalCountDisplay()
        {
            var capacityWorkDaysAndHours = (from player in Players
                                            from workDay in player.Capacity.WorkDays
                                            select (WorkDay: workDay, DailyHourCount: player.Capacity.DailyHourCount))
                .ToList();

            var capacityHoursTotalCount = capacityWorkDaysAndHours.Sum(x => x.DailyHourCount);
            var capacityHoursTotalCountDisplay = capacityHoursTotalCount.ToString(Formats.NumberNoDecimals);

            var remainingCount = capacityWorkDaysAndHours.Where(x => x.WorkDay.Date >= DateTime.UtcNow.Date)
                .Sum(x => x.DailyHourCount);
            if (remainingCount <= 0 || remainingCount == capacityHoursTotalCount)
            {
                return $"{capacityHoursTotalCountDisplay}h";
            }

            var remainingCountDisplay = remainingCount.ToString(Formats.NumberNoDecimals);

            return $"{remainingCountDisplay}/{capacityHoursTotalCountDisplay}h";
        }

        private string GetTotalWorkDayCountDisplay()
        {
            var capacityWorkDays = (from player in Players
                                    from workDay in player.Capacity.WorkDays
                                    select (WorkDay: workDay, DailyPercentMultiplier: player.Capacity.DailyPercent
                                                                                      / 100D)).ToList();

            var capacityWorkDaysCount = capacityWorkDays.Sum(x => x.DailyPercentMultiplier);
            var capacityWorkDaysCountDisplay = capacityWorkDaysCount.ToString(Formats.NumberNoDecimals);

            var remainingCount = capacityWorkDays.Where(x => x.WorkDay.Date >= DateTime.UtcNow.Date)
                .Sum(x => x.DailyPercentMultiplier);
            if (remainingCount <= 0 || remainingCount == capacityWorkDaysCount)
            {
                return $"{capacityWorkDaysCountDisplay}d";
            }

            var remainingCountDisplay = remainingCount.ToString(Formats.NumberNoDecimals);

            return $"{remainingCountDisplay}/{capacityWorkDaysCountDisplay}d";
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