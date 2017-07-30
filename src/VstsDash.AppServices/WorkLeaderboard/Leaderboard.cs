using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.RestApi.ApiResponses;
using VstsDash.AppServices.WorkIteration;

namespace VstsDash.AppServices.WorkLeaderboard
{
    public class Leaderboard
    {
        public const string WorkItemAssistTagName = "lb-assist";

        public const string WorkItemExcludeTagName = "lb-exclude";

        public Leaderboard(
            IReadOnlyCollection<TeamMemberApiResponse> teamMembers,
            IterationApiResponse iteration,
            IterationCapacityListApiResponse capacities,
            IterationDaysOffApiResponse teamDaysOff,
            Iteration workIteration)
        {
            if (teamMembers == null) throw new ArgumentNullException(nameof(teamMembers));
            if (iteration == null) throw new ArgumentNullException(nameof(iteration));
            if (capacities == null) throw new ArgumentNullException(nameof(capacities));
            if (teamDaysOff == null) throw new ArgumentNullException(nameof(teamDaysOff));
            if (workIteration == null) throw new ArgumentNullException(nameof(workIteration));

            IterationName = iteration.Name;

            var workItems = GetWorkItems(workIteration);

            var scores = LeaderboardScoresHelper.GetScores(iteration, workItems, teamMembers);

            var teamDaysOffDates = teamDaysOff
                .DaysOff
                .SelectMany(x => x.Start.GetDatesUntil(x.End))
                .Distinct()
                .ToList();

            var leaderboardTeamMembers = GetPlayers(
                iteration,
                teamMembers,
                capacities.Value,
                teamDaysOffDates,
                scores);

            IterationCapacity = new LeaderboardCapacity(iteration, teamDaysOffDates);

            Players = new ReadOnlyCollection<Player>(leaderboardTeamMembers);

            UnassignedScore = scores.Where(x => x.Key == Guid.Empty).Select(x => x.Value).FirstOrDefault();

            TotalScoreAssistsSum = Players.Sum(x => x.ScoreAssistsSum);
            TotalScoreGoalsSum = Players.Sum(x => x.ScoreGoalsSum);
            TotalScorePointsSum = Players.Sum(x => x.ScorePointsSum);

            TotalHoursTotalCount = Players.Sum(x => x.Capacity.HoursTotalCount);
            TotalWorkDaysTotalCount = Players.Sum(x => x.Capacity.WorkDaysTotalCount);
        }

        public LeaderboardCapacity IterationCapacity { get; }

        public string IterationName { get; }

        public IReadOnlyCollection<Player> Players { get; }

        public double TotalHoursTotalCount { get; }

        public double TotalScoreAssistsSum { get; }

        public double TotalScoreGoalsSum { get; }

        public double TotalScorePointsSum { get; }

        public double TotalWorkDaysTotalCount { get; }

        public Score UnassignedScore { get; }

        private static IList<Player> GetPlayers(
            IterationApiResponse iteration,
            IEnumerable<TeamMemberApiResponse> teamMembers,
            IEnumerable<IterationCapacityApiResponse> capacities,
            IEnumerable<DateTime> teamDaysOff,
            IDictionary<Guid, Score> scores)
        {
            return GetPlayersInternal(iteration, teamMembers, capacities, teamDaysOff, scores)
                .Where(x => x.Capacity.DailyHourCount > 0 || x.ScorePointsSum > 0)
                .OrderByDescending(x => x.ScorePointsSum)
                .ThenByDescending(x => x.ScoreGoalsSum)
                .ThenByDescending(x => x.ScoreAssistsSum)
                .ThenBy(x => x.ScorePointsSum > 0 ? x.Capacity.HoursTotalCount : 0)
                .ThenBy(x => x.ScorePointsSum > 0 ? x.Capacity.DailyHourCount : 0)
                .ThenBy(x => x.DisplayName)
                .ThenBy(x => x.UniqueName)
                .ToList();
        }

        private static IEnumerable<Player> GetPlayersInternal(
            IterationApiResponse iteration,
            IEnumerable<TeamMemberApiResponse> teamMembers,
            IEnumerable<IterationCapacityApiResponse> capacities,
            IEnumerable<DateTime> teamDaysOff,
            IDictionary<Guid, Score> scores)
        {
            return from teamMember in teamMembers
                let memberCapacity = capacities.FirstOrDefault(x => x.TeamMember.Id == teamMember.Id)
                let memberScore = scores.Where(x => x.Key == teamMember.Id).Select(x => x.Value).FirstOrDefault()
                select new Player(
                    teamMember,
                    iteration,
                    teamDaysOff,
                    memberCapacity,
                    memberScore);
        }

        private static IEnumerable<WorkItem> GetWorkItems(Iteration workIteration)
        {
            var workItems = workIteration.Items
                .Where(x => !x.Tags.Contains(WorkItemExcludeTagName, StringComparer.OrdinalIgnoreCase))
                .ToList();

            foreach (var item in workItems)
                item.ChildItems = item.ChildItems
                    .Where(c => !c.Tags.Contains(WorkItemExcludeTagName, StringComparer.OrdinalIgnoreCase))
                    .ToList();

            return workItems;
        }
    }
}