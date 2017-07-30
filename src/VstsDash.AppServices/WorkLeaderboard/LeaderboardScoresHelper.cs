using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.RestApi.ApiResponses;
using VstsDash.AppServices.WorkIteration;

namespace VstsDash.AppServices.WorkLeaderboard
{
    internal static class LeaderboardScoresHelper
    {
        private const double MaxGoalValue = 3;

        public static IDictionary<Guid, Score> GetScores(
            IterationApiResponse iteration,
            IEnumerable<WorkItem> workItems,
            IReadOnlyCollection<TeamMemberApiResponse> teamMembers)
        {
            var points = GetPoints(iteration, workItems, teamMembers)
                .Where(x => x.Id == Guid.Empty || x.Point.Value > 0)
                .OrderByDescending(x => x.Point.EarnedAt)
                .ThenByDescending(x => x.Point.Value)
                .ThenBy(x => x.Point.Description)
                .ThenByDescending(x => x.Point.Type)
                .GroupBy(x => x.Id, x => x.Point)
                .ToList();

            var scores = points.ToDictionary(x => x.Key, x => new Score(x));
            return scores;
        }

        private static IEnumerable<(Guid Id, Point Point)> GetPoints(
            IterationApiResponse iteration,
            IEnumerable<WorkItem> workItems,
            IReadOnlyCollection<TeamMemberApiResponse> teamMembers)
        {
            var doneWorkItems = workItems.Where(x => x.IsStateDone && x.IterationPath == iteration.Path).ToList();

            var goals = GetGoals(doneWorkItems)
                .Distinct(x => new
                {
                    x.Id,
                    x.Point.Type,
                    x.Point.Description,
                    x.Point.Value
                })
                .ToList();

            foreach (var goal in goals)
                yield return teamMembers.Any(x => x.Id == goal.Id) ? goal : (Guid.Empty, goal.Point);

            foreach (var workItem in doneWorkItems)
            {
                var doneChildItems = workItem
                    .ChildItems
                    .Where(x => x.IsStateDone
                                && x.IterationPath == iteration.Path
                                && x.AssignedToMember?.Id != workItem.AssignedToMember?.Id)
                    .ToList();

                var value = GetAssistValue(workItem);

                var maxCount = GetAssistMaxCount(workItem);

                var assists = GetAssists(workItem, doneChildItems, value, maxCount)
                    .Distinct(x => new
                    {
                        x.Id,
                        x.Point.Type,
                        x.Point.Description,
                        x.Point.Value
                    })
                    .ToList();

                foreach (var assist in assists)
                    yield return teamMembers.Any(x => x.Id == assist.Id) ? assist : (Guid.Empty, assist.Point);
            }
        }

        private static IEnumerable<(Guid Id, Point Point)> GetGoals(IEnumerable<WorkItem> doneWorkItems)
        {
            foreach (var workItem in doneWorkItems)
            {
                var value = GetGoalValue(workItem);
                var id = Convert.ToString(workItem.Id);
                var description = GetPointDescription(workItem);
                var earnedAt = GetPointEarnedAt(workItem);

                var assignedToId = (value > 0 ? workItem.AssignedToMember?.Id : null) ?? Guid.Empty;

                yield return (assignedToId, new Point(TeamMemberPointType.Goal, value, id, description, earnedAt));
            }
        }

        private static IEnumerable<(Guid Id, Point Point)> GetAssists(
            WorkItem workItem,
            IEnumerable<WorkItem> doneChildItems,
            double value,
            int maxCount)
        {
            if (maxCount <= 0)
                yield break;

            var candidates = (from child in doneChildItems
                let assignedToMemberId = child.AssignedToMember?.Id ?? Guid.Empty
                group child by assignedToMemberId
                into g
                select new
                {
                    g.Key,
                    Count = g.Count(),
                    HasAssistTag = g.Any(x => x.Tags.Contains(Leaderboard.WorkItemAssistTagName,
                        StringComparer.OrdinalIgnoreCase)),
                    WorkItem = g.First()
                }).OrderByDescending(x => x.HasAssistTag).ThenByDescending(x => x.Count).Take(maxCount + 1).ToList();

            if (!candidates.Any())
                yield break;

            var id = Convert.ToString(workItem.Id);
            var description = GetPointDescription(workItem);

            var candidateGroups = candidates.GroupBy(x => new
            {
                x.HasAssistTag,
                x.Count
            }).ToList();

            var totalTopItems = 0;
            foreach (var candidateGroup in candidateGroups)
            {
                totalTopItems += candidateGroup.Count();

                if (totalTopItems <= maxCount)
                {
                    var earnedAt = candidateGroup
                        .Select(x => GetPointEarnedAt(x.WorkItem))
                        .OrderByDescending(x => x)
                        .FirstOrDefault();

                    foreach (var item in candidateGroup)
                        yield return (item.Key, new Point(TeamMemberPointType.Assist, value, id, description,
                            earnedAt));

                    if (totalTopItems == maxCount)
                        yield break;
                }
                else
                {
                    var earnedAt = GetPointEarnedAt(workItem);

                    yield return (Guid.Empty, new Point(TeamMemberPointType.Assist, value, id, description, earnedAt));
                    yield break;
                }
            }
        }

        private static int GetAssistMaxCount(WorkItem workItem)
        {
            var goalValue = GetGoalValue(workItem);

            if (goalValue <= 1)
                return 0;

            return goalValue >= 3 ? 2 : 1;
        }

        private static double GetAssistValue(WorkItem workItem)
        {
            var goalValue = GetGoalValue(workItem);

            if (goalValue <= 1)
                return 0;

            return goalValue >= 3 ? 1 : 0.5;
        }

        private static double GetGoalValue(WorkItem workItem)
        {
            return workItem.Effort.Clamp(0, MaxGoalValue);
        }

        private static string GetPointDescription(WorkItem workItem)
        {
            return workItem.Title;
        }

        private static DateTime GetPointEarnedAt(WorkItem workItem)
        {
            return workItem.ClosedDate ?? workItem.ChangedDate ?? workItem.CreatedDate ?? DateTime.MinValue;
        }
    }
}