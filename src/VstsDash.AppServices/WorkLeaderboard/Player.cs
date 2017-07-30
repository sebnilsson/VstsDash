using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkLeaderboard
{
    public class Player
    {
        public Player(
            TeamMemberApiResponse teamMember,
            IterationApiResponse iteration,
            IEnumerable<DateTime> teamDaysOff,
            IterationCapacityApiResponse capacity = null,
            Score score = null)
        {
            if (teamMember == null) throw new ArgumentNullException(nameof(teamMember));
            if (iteration == null) throw new ArgumentNullException(nameof(iteration));
            if (teamDaysOff == null) throw new ArgumentNullException(nameof(teamDaysOff));

            DisplayName = teamMember.DisplayName;
            Id = teamMember.Id;
            ImageUrl = teamMember.ImageUrl;
            UniqueName = teamMember.UniqueName;

            Capacity = new LeaderboardCapacity(iteration, teamDaysOff, capacity);

            Score = score ?? Score.Empty;

            ScoreAssistsSum = GetScoreSum(Score.Assists);
            ScoreGoalsSum = GetScoreSum(Score.Goals);
            ScorePointsSum = GetScoreSum(Score.Points);
        }

        public LeaderboardCapacity Capacity { get; }

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
            return points.Sum(x => x.Value * Capacity.Multiplier);
        }
    }
}