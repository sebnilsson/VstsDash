using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class Iteration
    {
        public Iteration(
            WorkItemListApiResponse workItems,
            IReadOnlyCollection<TeamMemberApiResponse> teamMembers,
            IterationApiResponse iteration,
            IterationCapacityListApiResponse capacities,
            bool isBacklog)
        {
            if (workItems == null) throw new ArgumentNullException(nameof(workItems));
            if (teamMembers == null) throw new ArgumentNullException(nameof(teamMembers));
            if (iteration == null) throw new ArgumentNullException(nameof(iteration));

            IsBacklog = isBacklog;

            IterationId = iteration.Id;
            IterationName = iteration.Name;
            IterationPath = iteration.Path;
            IterationStartDate = iteration.Attributes?.StartDate;
            IterationFinishDate = iteration.Attributes?.FinishDate;

            IterationDayCount = IterationStartDate != null && IterationFinishDate != null
                                    ? (IterationFinishDate.Value - IterationStartDate.Value).TotalDays + 1
                                    : 0;

            var items = workItems.Value.Select(x => new WorkItem(x, teamMembers))
                .OrderBy(x => x.BacklogPriority)
                .ToList();

            foreach (var item in items)
            {
                var parent = item.ParentId > 0 ? items.FirstOrDefault(x => x.Id == item.ParentId) : null;

                if (parent != null) parent.ChildItems.Add(item);
                else Items.Add(item);
            }

            //this.Items = this.Items.Where(x => x.IsTypeProductBacklogItem || x.IsTypeBug || x.IsTypeTask).ToList();

            Items = Items.OrderByDescending(x => x.BacklogPriority > 0).ThenBy(x => x.BacklogPriority).ToList();

            Items.ToList().ForEach(x => x.IsTopLevel = true);

            if (capacities != null)
            {
                var groupedCapacities = from capacity in capacities.Value

                                        //from activity in capacity.Activities
                                        //from daysOff in capacity.DaysOff
                                        group capacity by capacity.TeamMember.Id
                                        into g
                                        select new
                                               {
                                                   TeamMember = g.Select(x => x.TeamMember).First(),
                                                   Activities = g.SelectMany(x => x.Activities),
                                                   DaysOff = g.SelectMany(x => x.DaysOff)
                                               };

                Capacities = groupedCapacities.Select(x => new Capacity(x.TeamMember, x.Activities, x.DaysOff))
                    .ToList();
            }
        }

        public IEnumerable<WorkItem> AllChildItems => GetAllChildItems();

        public ICollection<Capacity> Capacities { get; set; } = new List<Capacity>();

        public double EffortTotal => Items?.Sum(x => x.EffortTotal) ?? 0;

        public bool IsBacklog { get; set; }

        public ICollection<WorkItem> Items { get; set; } = new List<WorkItem>();

        public double IterationDayCount { get; set; }

        public DateTime? IterationFinishDate { get; set; }

        public Guid IterationId { get; set; }

        public string IterationName { get; set; }

        public string IterationPath { get; set; }

        public DateTime? IterationStartDate { get; set; }

        public double RemainingWorkInProgress => AllChildItems.Where(x => x.IsStateInProgress)
            .Sum(x => x.RemainingWork);

        public double RemainingWorkInProgressCount => AllChildItems.Count(x => x.IsStateInProgress);

        public double RemainingWorkToDo => AllChildItems.Where(x => x.IsStateToDo).Sum(x => x.RemainingWork);

        public double RemainingWorkToDoCount => AllChildItems.Count(x => x.IsStateToDo);

        public double RemainingWorkTotal => Items?.Sum(x => x.RemainingWorkTotal) ?? 0;

        private IEnumerable<WorkItem> GetAllChildItems()
        {
            return Items?.SelectMany(x => x.ChildItems ?? Enumerable.Empty<WorkItem>());
        }
    }
}