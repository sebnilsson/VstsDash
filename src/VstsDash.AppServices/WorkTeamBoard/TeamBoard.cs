﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkTeamBoard
{
    public class TeamBoard
    {
        public const string WorkItemAssistTagName = "dash-assist";
        public const string WorkItemBonusTagName = "dash-bonus";
        public const string WorkItemExcludeTagName = "dash-exclude";

        public TeamBoard(
            TeamMemberListApiResponse teamMembers,
            IterationApiResponse iteration,
            IterationCapacityListApiResponse capacities,
            IterationDaysOffApiResponse teamDaysOff,
            Iteration workIteration)
        {
            if (teamMembers == null)
                throw new ArgumentNullException(nameof(teamMembers));
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration));
            if (capacities == null)
                throw new ArgumentNullException(nameof(capacities));
            if (teamDaysOff == null)
                throw new ArgumentNullException(nameof(teamDaysOff));
            if (workIteration == null)
                throw new ArgumentNullException(nameof(workIteration));

            IterationName = iteration.Name;

            var teamMemberList = teamMembers.Value.ToList();

            var teamCapacity = new TeamCapacity(iteration, teamDaysOff, teamMembers, capacities);

            var workItems = GetWorkItems(workIteration);

            var scores = TeamBoardScoresHelper.GetScores(iteration, workItems, teamMemberList);

            var teamBoardTeamMembers = GetPlayers(teamMemberList, teamCapacity, scores);

            Players = new ReadOnlyCollection<Player>(teamBoardTeamMembers);
            TeamCapacity = teamCapacity;

            UnassignedScore = scores.Where(x => x.Key == Guid.Empty).Select(x => x.Value).FirstOrDefault();

            TotalScoreAssistsSum = Players.Sum(x => x.ScoreAssistsSum);
            TotalScoreGoalsSum = Players.Sum(x => x.ScoreGoalsSum);
            TotalScorePointsSum = Players.Sum(x => x.ScorePointsSum);

            TotalHoursTotalCount = Players.Sum(x => x.Capacity.HoursTotalCount);
            TotalWorkDayCount = Players.Sum(x => x.Capacity.TotalWorkDayCount);
        }

        public string IterationName { get; }

        public IReadOnlyCollection<Player> Players { get; }

        public TeamCapacity TeamCapacity { get; }

        public double TotalHoursTotalCount { get; }

        public double TotalScoreAssistsSum { get; }

        public double TotalScoreGoalsSum { get; }

        public double TotalScorePointsSum { get; }

        public double TotalWorkDayCount { get; }

        public Score UnassignedScore { get; }

        private static IList<Player> GetPlayers(
            IEnumerable<TeamMemberApiResponse> teamMembers,
            TeamCapacity teamCapacity,
            IDictionary<Guid, Score> scores)
        {
            var random = new Random();

            return GetPlayersInternal(teamMembers, teamCapacity, scores)
                .Where(x => x.Capacity.HasAnyActivities || x.ScorePointsSum > 0)
                .OrderByDescending(x => x.ScorePointsSum)
                .ThenByDescending(x => x.ScoreGoalsSum)
                .ThenByDescending(x => x.ScoreAssistsSum)
                .ThenByDescending(x => x.Score.Points.Count(s => s.HasBonus))
                .ThenBy(x => x.ScorePointsSum > 0 ? x.Capacity.HoursTotalCount : 0)
                .ThenBy(x => x.ScorePointsSum > 0 ? x.Capacity.DailyHourCount : 0)
                .ThenBy(_ => random.Next())
                .ToList();
        }

        private static IEnumerable<Player> GetPlayersInternal(
            IEnumerable<TeamMemberApiResponse> teamMembers,
            TeamCapacity teamCapacity,
            IDictionary<Guid, Score> scores)
        {
            return from teamMember in teamMembers
                let memberCapacity = teamCapacity.Members.FirstOrDefault(x => x.MemberId == teamMember.Id)
                let memberScore = scores.Where(x => x.Key == teamMember.Id).Select(x => x.Value).FirstOrDefault()
                select new Player(teamMember, teamCapacity, memberCapacity, memberScore);
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