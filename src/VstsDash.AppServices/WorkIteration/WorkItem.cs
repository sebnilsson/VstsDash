using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class WorkItem
    {
        public WorkItem(WorkItemApiResponse workItem, IReadOnlyCollection<TeamMemberApiResponse> teamMembers)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));
            if (teamMembers == null)
                throw new ArgumentNullException(nameof(teamMembers));

            ChildItems = new List<WorkItem>();

            Id = workItem.Id;

            if (workItem.Fields != null)
            {
                AreaPath = workItem.Fields.AreaPath;
                AssignedTo = workItem.Fields.AssignedTo;
                BacklogPriority = workItem.Fields.BacklogPriority;
                BoardColumn = workItem.Fields.BoardColumn;
                ChangedBy = workItem.Fields.ChangedBy;
                ChangedDate = workItem.Fields.ChangedDate;
                ClosedDate = workItem.Fields.ClosedDate;
                CreatedBy = workItem.Fields.CreatedBy;
                CreatedDate = workItem.Fields.CreatedDate;
                Description = workItem.Fields.Description;
                Effort = workItem.Fields.Effort;
                IterationPath = workItem.Fields.IterationPath;
                Priority = workItem.Fields.Priority;
                RemainingWork = workItem.Fields.RemainingWork;
                State = workItem.Fields.State;
                Tags = workItem.Fields.TagList;
                TeamProject = workItem.Fields.TeamProject;
                Title = workItem.Fields.Title;
                ValueArea = workItem.Fields.ValueArea;
                WorkItemType = workItem.Fields.WorkItemType;

                AssignedToMember = GetMemberFromString(AssignedTo, teamMembers);
                ChangedByMember = GetMemberFromString(ChangedBy, teamMembers);
                CreatedByMember = GetMemberFromString(CreatedBy, teamMembers);
            }
            else
            {
                Tags = Enumerable.Empty<string>();
            }

            if (workItem.Relations != null)
            {
                var parentRelation = workItem.Relations.FirstOrDefault(x => x.IsRelHierarchyReverse);
                ParentId = parentRelation?.UrlId ?? 0;
            }
        }

        public long Id { get; set; }

        public string AreaPath { get; set; }

        public string AssignedTo { get; set; }

        public TeamMember AssignedToMember { get; set; }

        public double BacklogPriority { get; set; }

        public string BoardColumn { get; set; }

        public string ChangedBy { get; set; }

        public DateTime? ChangedDate { get; set; }

        public TeamMember ChangedByMember { get; set; }

        public IList<WorkItem> ChildItems { get; set; }

        public DateTime? ClosedDate { get; set; }

        public string CreatedBy { get; set; }

        public TeamMember CreatedByMember { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Description { get; set; }

        public double Effort { get; set; }

        public double EffortTotal => (ChildItems?.Sum(x => x.Effort) ?? 0) + Effort;

        public bool HasChildren => ChildItems?.Any() ?? false;

        public bool IsTopLevel { get; set; }

        public bool IsStateCommitted => WorkItemState.IsCommited(State);

        public bool IsStateDone => WorkItemState.IsDone(State);

        public bool IsStateToDo => WorkItemState.IsToDo(State);

        public bool IsStateInProgress => WorkItemState.IsInProgress(State);

        public bool IsTypeBug => "Bug".Equals(WorkItemType, StringComparison.OrdinalIgnoreCase);

        public bool IsTypeProductBacklogItem
            => "Product Backlog Item".Equals(WorkItemType, StringComparison.OrdinalIgnoreCase);

        public bool IsTypeTask => "Task".Equals(WorkItemType, StringComparison.OrdinalIgnoreCase);

        public string IterationPath { get; set; }

        public int ParentId { get; set; }

        public int Priority { get; set; }

        public double RemainingWork { get; set; }

        public double RemainingWorkTotal => (ChildItems?.Sum(x => x.RemainingWork) ?? 0) + RemainingWork;

        public string State { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string TeamProject { get; set; }

        public string Title { get; set; }

        public string ValueArea { get; set; }

        public string WorkItemType { get; set; }

        private static TeamMember GetMemberFromString(string value, IEnumerable<TeamMemberApiResponse> teamMembers)
        {
            var matchValue = value?.ToLowerInvariant() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(matchValue))
                return null;

            foreach (var member in teamMembers)
            {
                var matchUniqueName = $"<{member.UniqueName}>".ToLowerInvariant();

                if (matchValue.Contains(matchUniqueName))
                    return new TeamMember(member);
            }

            return null;
        }
    }
}