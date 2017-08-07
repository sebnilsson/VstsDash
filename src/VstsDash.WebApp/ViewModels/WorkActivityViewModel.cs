using System;
using System.Collections.Generic;
using System.Linq;

namespace VstsDash.WebApp.ViewModels
{
    public class WorkActivityViewModel
    {
        public IReadOnlyCollection<ActivityTeamCapacity.ActivityTeamMemberCapacity> ActiveMemberCapacities =>
            TeamCapacity.Members.Where(x => x.DailyPercent > 0).ToList();

        public IReadOnlyCollection<AuthorCommits> Authors { get; set; }

        public int AuthorsCommitsTotalChangeCountSum => Authors.Sum(ac => ac.CommitsTotalChangeCountSum);

        public int AuthorsCommitCountSum => Authors.Sum(ac => ac.CommitCount);

        public IReadOnlyCollection<CommitInfo> Commits { get; set; }

        public IReadOnlyList<KeyValuePair<DateTime, double?>> EffortDone { get; set; }

        public double? EffortDonePerDay => GetEffortDonePerDay();

        public double? EffortDonePerMember => GetEffortDonePerMember();

        public double FullMemberCount => GetFullMemberCount();

        public DateTime FromDate { get; set; }

        public string IterationName { get; set; }

        public double? LastEffortDone => EffortDone
            .Where(x => x.Value != null)
            .OrderByDescending(x => x.Key)
            .Select(x => x.Value)
            .FirstOrDefault();

        public double? MaxEffortDonePerDay => GetMaxEffortDonePerDay();

        public IReadOnlyCollection<RepositoryAuthors> Repos { get; set; }

        public ActivityTeamCapacity TeamCapacity { get; set; }

        public DateTime ToDate { get; set; }

        private double? GetEffortDonePerDay()
        {
            if (LastEffortDone == null || LastEffortDone <= 0)
                return LastEffortDone;

            var pastWorkDayCount = ActiveMemberCapacities.Any()
                ? ActiveMemberCapacities.Sum(c => c.WorkDays.Count(w => w.Date <= DateTimeOffset.UtcNow.Date))
                : 0;

            return pastWorkDayCount > 0 ? LastEffortDone / pastWorkDayCount : null;
        }

        private double? GetEffortDonePerMember()
        {
            if (LastEffortDone == null || LastEffortDone <= 0)
                return LastEffortDone;

            return FullMemberCount >= 0 ? LastEffortDone / FullMemberCount : null;
        }

        private double GetFullMemberCount()
        {
            var memberFullCapacity = TeamCapacity.WorkDays.Count * 8;

            return ActiveMemberCapacities.Sum(x => x.WorkDays.Count * x.DailyHourCount / memberFullCapacity);
        }

        private double? GetMaxEffortDonePerDay()
        {
            if (!EffortDone.Any())
                return null;

            var donePerDay = EffortDone
                .Select((x, i) => (Value: x, Difference: i > 0 ? x.Value - EffortDone[i - 1].Value : -1))
                .Where(x => x.Difference >= 0);
            return donePerDay.Max(x => x.Difference);
        }

        public class Author
        {
            public Guid MemberId { get; set; }

            public string MemberDisplayName { get; set; }

            public string MemberImageUrl { get; set; }

            public string MemberUniqueName { get; set; }
        }

        public class AuthorCommits
        {
            public double? AverageChangePerCommit => CommitCount > 0
                ? CommitsTotalChangeCountSum / (double) CommitCount
                : (double?) null;

            public Author Author { get; set; }

            public int CommitCount => Commits.Count;

            public IReadOnlyCollection<CommitInfo> Commits { get; set; }

            public int CommitsTotalChangeCountSum => Commits.Sum(x => x.Commit.TotalChangeCount);

            private IEnumerable<CommitInfo> KnownAuthorCommits => Commits.Where(x => x.Author.MemberId != Guid.Empty);

            public int? MaxChangePerCommit => KnownAuthorCommits.Any()
                ? KnownAuthorCommits.Max(x => x.Commit.TotalChangeCount)
                : (int?) null;

            public int? MaxChangePerDay => KnownAuthorCommits.Any()
                ? KnownAuthorCommits
                    .GroupBy(x => (x.Commit.AuthorDate ?? DateTime.MinValue).Date)
                    .Max(g => g.Sum(x => x.Commit.TotalChangeCount))
                : (int?) null;

            public int? MaxCommitsPerDay => KnownAuthorCommits.Any()
                ? KnownAuthorCommits.GroupBy(c => (c.Commit.AuthorDate ?? DateTime.MinValue).Date).Max(g => g.Count())
                : (int?) null;
        }

        public class ActivityTeamCapacity
        {
            public double DailyHourCount { get; set; }

            public double DailyPercent { get; set; }

            public double HoursTotalCount { get; set; }

            public IReadOnlyCollection<DateTime> IterationWorkDays { get; set; }

            public IReadOnlyCollection<ActivityTeamMemberCapacity> Members { get; set; }

            public IReadOnlyCollection<DateTime> TeamDaysOff { get; set; }

            public IReadOnlyCollection<DateTime> WorkDays { get; set; }

            public double TotalWorkDayCount { get; set; }

            public class ActivityTeamMemberCapacity
            {
                public IReadOnlyCollection<DateTime> DaysOff { get; set; }

                public double DailyHourCount { get; set; }

                public double DailyPercent { get; set; }

                public double HoursTotalCount { get; set; }

                public IReadOnlyCollection<DateTime> MemberDaysOff { get; set; }

                public Guid MemberId { get; set; }

                public double TotalWorkDayCount { get; set; }

                public IReadOnlyCollection<DateTime> WorkDays { get; set; }
            }
        }

        public class Commit
        {
            public int AddChangeCount { get; set; }

            public DateTimeOffset? AuthorDate { get; set; }

            public string AuthorEmail { get; set; }

            public string AuthorName { get; set; }

            public string Comment { get; set; }

            public string CommitId { get; set; }

            public DateTimeOffset? CommitterDate { get; set; }

            public string CommitterEmail { get; set; }

            public string CommitterName { get; set; }

            public int DeleteChangeCount { get; set; }

            public int EditChangeCount { get; set; }

            public int TotalChangeCount { get; set; }
        }

        public class CommitInfo
        {
            public Author Author { get; set; }

            public Commit Commit { get; set; }

            public Repository Repository { get; set; }
        }

        public class Repository
        {
            public Guid RepositoryId { get; set; }

            public string RepositoryName { get; set; }
        }

        public class RepositoryAuthors
        {
            public IReadOnlyCollection<AuthorCommits> AuthorCommits { get; set; }

            public Repository Repository { get; set; }

            public int AuthorCommitsCountSum => AuthorCommits.Sum(x => x.CommitCount);
        }
    }
}