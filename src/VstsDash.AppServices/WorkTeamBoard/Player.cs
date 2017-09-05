using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkTeamBoard
{
    public class Player
    {
        private const int MaxCapacityMultiplier = 4;

        public Player(
            TeamMemberApiResponse teamMember,
            TeamCapacity teamCapacity,
            TeamMemberCapacity memberCapacity = null,
            Score score = null)
        {
            if (teamMember == null)
                throw new ArgumentNullException(nameof(teamMember));
            if (teamCapacity == null)
                throw new ArgumentNullException(nameof(teamCapacity));

            DisplayName = teamMember.DisplayName;
            Id = teamMember.Id;
            ImageUrl = teamMember.ImageUrl;
            UniqueName = teamMember.UniqueName;

            Capacity = memberCapacity ?? TeamMemberCapacity.Default(teamMember.Id, teamCapacity);

            CapacityMultiplier = GetCapacityMultiplier(teamCapacity, memberCapacity);

            Score = score ?? Score.Empty;

            ScoreAssistsSum = GetScoreSum(Score.Assists);
            ScoreGoalsSum = GetScoreSum(Score.Goals);
            ScorePointsSum = GetScoreSum(Score.Points);
        }

        public TeamMemberCapacity Capacity { get; }

        public double CapacityMultiplier { get; }

        public string DisplayName { get; }

        public Guid Id { get; }

        public string ImageUrl { get; }

        public Score Score { get; }

        public double ScoreAssistsSum { get; }

        public double ScoreGoalsSum { get; }

        public double ScorePointsSum { get; }

        public string UniqueName { get; }

        private static double GetCapacityMultiplier(TeamCapacity teamCapacity, TeamMemberCapacity memberCapacity)
        {
            var fullCapacityHours = teamCapacity.WorkDays.Count * 8D;

            var memberCapcityHours = memberCapacity.WorkDays.Count * (memberCapacity.DailyPercent / 100D * 8D);

            return (fullCapacityHours / memberCapcityHours).Clamp(1, MaxCapacityMultiplier);
        }

        private double GetScoreSum(IEnumerable<Point> points)
        {
            var goals = points.Where(x => x.Type == TeamMemberPointType.Goal).ToList(); ;

            var otherPoints = points.Except(goals).ToList();

            var goalsValue = goals.Sum(
                x =>
                {
                    var value = x.HasBonus ? (x.Value + 1) : x.Value;
                    return value * CapacityMultiplier;
                });

            var otherPointsValue = otherPoints.Sum(x => x.HasBonus ? (x.Value + 1) : x.Value);

            var roundedValues = (goalsValue + otherPointsValue).RoundToHalfs();
            return roundedValues;
        }
    }
}