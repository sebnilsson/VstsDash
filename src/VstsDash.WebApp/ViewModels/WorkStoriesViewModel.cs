using System;
using System.Collections.Generic;
using System.Linq;

namespace VstsDash.WebApp.ViewModels
{
    public class WorkStoriesViewModel
    {
        public IReadOnlyCollection<Story> AllParentWorkItems { get; set; }

        public double CapacityTotal => GetCapacityTotal();

        public double CapacityRemaining => GetCapacityRemaining();

        public int CapacityRemainingPercent => GetCapacityRemainingPercent();

        public DateTime FromDate { get; set; }

        public string IterationName { get; set; }

        public IReadOnlyCollection<Story> Stories { get; set; }

        public IReadOnlyCollection<Story> StoriesDone => GetStoriesDone();

        public IReadOnlyCollection<Story> StoriesInProgress => GetStoriesInProgress();

        public double EffortsInProgress => GetEffortsInProgress();

        public int EffortsInProgressPercent => GetEffortsInProgressPercent();

        public double EffortsDone => GetEffortsDone();

        public int EffortsDonePercent => GetEffortsDonePercent();

        public double EffortsTotal => GetEffortsTotal();

        public WorkStoriesTeamCapacity TeamCapacity { get; set; }

        public DateTime ToDate { get; set; }

        private int GetCapacityRemainingPercent()
        {
            if (CapacityTotal <= 0)
                return -1;
            if (CapacityRemaining >= CapacityTotal)
                return 0;
            if (CapacityRemaining <= 0)
                return 100;

            var percent = CapacityRemaining > 0 ? 100 - (int) (CapacityRemaining / CapacityTotal * 100) : 0;
            return percent.Clamp(0, 100);
        }

        private double GetCapacityTotal()
        {
            return TeamCapacity.HoursTotalCount;
        }

        private double GetCapacityRemaining()
        {
            return (from member in TeamCapacity.Members
                let daysLeft = member.WorkDays.Count(x => x >= DateTime.UtcNow.Date)
                select daysLeft * member.DailyHourCount).Sum();
        }

        private IReadOnlyCollection<Story> GetStoriesDone()
        {
            return Stories.Where(x => x.State == Story.StoryState.Done).ToList();
        }

        private IReadOnlyCollection<Story> GetStoriesInProgress()
        {
            return Stories.Where(x => x.State == Story.StoryState.InProgress).ToList();
        }

        private double GetEffortsDone()
        {
            return AllParentWorkItems.Where(x => x.State == Story.StoryState.Done).Sum(x => x.Effort);
        }

        private int GetEffortsDonePercent()
        {
            var percent = EffortsTotal > 0 ? (int) (EffortsDone / EffortsTotal * 100) : 0;
            return percent.Clamp(0, 100);
        }

        private double GetEffortsInProgress()
        {
            return AllParentWorkItems.Where(x => x.State == Story.StoryState.InProgress).Sum(x => x.Effort);
        }

        private int GetEffortsInProgressPercent()
        {
            var percent = EffortsTotal > 0 ? (int) (EffortsInProgress / EffortsTotal * 100) : 0;
            return percent.Clamp(0, 100);
        }

        private double GetEffortsTotal()
        {
            return AllParentWorkItems.Sum(x => x.Effort);
        }

        public class Story
        {
            public enum StoryState
            {
                NotStarted,
                InProgress,
                Done
            }

            public IReadOnlyCollection<(string Name, string MemberImageUrl)> Assists { get; set; }

            public DateTime? ChangedDate { get; set; }

            public DateTime? ClosedDate { get; set; }

            public DateTime? CreatedDate { get; set; }

            public double Effort { get; set; }

            public long Id { get; set; }

            public bool IsBug { get; set; }

            public bool IsStory { get; set; }

            public string MemberDisplayName { get; set; }

            public Guid MemberId { get; set; }

            public string MemberImageUrl { get; set; }

            public string MemberUniqueName { get; set; }

            public string Title { get; set; }

            public StoryState State { get; set; }
        }

        public class WorkStoriesTeamCapacity
        {
            public double DailyHourCount { get; set; }

            public double DailyPercent { get; set; }

            public double HoursTotalCount { get; set; }

            public IReadOnlyCollection<DateTime> IterationWorkDays { get; set; }

            public IReadOnlyCollection<WorkStoriesTeamCapacityMember> Members { get; set; }

            public IReadOnlyCollection<DateTime> TeamDaysOff { get; set; }

            public double TotalWorkDayCount { get; set; }

            public IReadOnlyCollection<DateTime> WorkDays { get; set; }

            public class WorkStoriesTeamCapacityMember
            {
                public double DailyHourCount { get; set; }

                public IReadOnlyCollection<DateTime> WorkDays { get; set; }
            }
        }
    }
}