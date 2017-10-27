using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.AppServices.WorkIteration;

namespace VstsDash.AppServices.WorkStories
{
    public class Story
    {
        public Story(WorkItem workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            ChangedDate = workItem.ChangedDate;
            ClosedDate = workItem.ClosedDate;
            CreatedDate = workItem.CreatedDate;
            Effort = workItem.Effort;
            Id = workItem.Id;
            MemberDisplayName = workItem.AssignedToMember.DisplayName;
            MemberId = workItem.AssignedToMember.Id;
            MemberImageUrl = workItem.AssignedToMember.ImageUrl;
            MemberUniqueName = workItem.AssignedToMember.UniqueName;
            Title = workItem.Title;

            State = GetState(workItem);
            Assists = GetAssists(workItem);
        }

        public DateTime? ChangedDate { get; }

        public DateTime? ClosedDate { get; }

        public DateTime? CreatedDate { get; }

        public IReadOnlyCollection<(string Name, string MemberImageUrl)> Assists { get; }

        public double Effort { get; }

        public long Id { get; }

        public string MemberDisplayName { get; }

        public Guid MemberId { get; }

        public string MemberImageUrl { get; }

        public string MemberUniqueName { get; }

        public string Title { get; set; }

        public StoryState State { get; set; }

        private static IReadOnlyCollection<(string Name, string MemberImageUrl)> GetAssists(WorkItem workItem)
        {
            var childItems = workItem.ChildItems ?? Enumerable.Empty<WorkItem>();

            return childItems
                .Where(x => x.IsTypeTask && x.AssignedToMember != null &&
                            x.AssignedToMember.Id != workItem.AssignedToMember.Id)
                .Distinct(x => x.AssignedToMember.Id)
                .OrderBy(x => x.AssignedToMember.DisplayName)
                .Select(x => (x.AssignedToMember.DisplayName, x.AssignedToMember.ImageUrl))
                .ToList();
        }

        private static StoryState GetState(WorkItem workItem)
        {
            if (workItem.IsStateDone)
                return StoryState.Done;

            var isInProgress = workItem.ChildItems.Any(x => x.IsStateInProgress);

            return isInProgress ? StoryState.InProgress : StoryState.NotStarted;
        }
    }
}