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
            if (teamMember == null) throw new ArgumentNullException(nameof(teamMember));
            if (teamCapacity == null) throw new ArgumentNullException(nameof(teamCapacity));

            DisplayName = teamMember.DisplayName;
            Id = teamMember.Id;
            ImageUrl = teamMember.ImageUrl;
            UniqueName = teamMember.UniqueName;

            Capacity = memberCapacity ?? TeamMemberCapacity.Default(teamMember.Id, teamCapacity);
            CapacityMultiplier =
                (Capacity.DailyPercent > 0 ? 100 / Capacity.DailyPercent : MaxCapacityMultiplier).Clamp(
                    1,
                    MaxCapacityMultiplier);

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

        private double GetScoreSum(IEnumerable<Point> points)
        {
            return points.Sum(x => x.Value * CapacityMultiplier);
        }
    }
}