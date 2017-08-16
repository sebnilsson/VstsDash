using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class TeamMemberCapacity
    {
        private const int FullCapacityDailyHourCount = 8;

        public TeamMemberCapacity(
            Guid memberId,
            IterationCapacityApiResponse capacity,
            IEnumerable<DateTime> iterationWorkDays,
            IEnumerable<DateTime> teamDaysOff,
            IEnumerable<DateTime> teamWorkDays)
            : this(memberId, iterationWorkDays, teamDaysOff, teamWorkDays, capacity)
        {
        }

        private TeamMemberCapacity(
            Guid memberId,
            IEnumerable<DateTime> iterationWorkDays,
            IEnumerable<DateTime> teamDaysOff,
            IEnumerable<DateTime> teamWorkDays,
            IterationCapacityApiResponse capacity = null)
        {
            if (iterationWorkDays == null) throw new ArgumentNullException(nameof(iterationWorkDays));
            if (teamDaysOff == null) throw new ArgumentNullException(nameof(teamDaysOff));
            if (teamWorkDays == null) throw new ArgumentNullException(nameof(teamWorkDays));

            MemberId = memberId;

            HasAnyActivities = capacity?.Activities?.Any() ?? false;

            var memberCapacityPerDay = GetMemberCapacityPerDay(capacity);
            var memberDaysOff = GetMemberDaysOff(capacity);

            var memberDaysOffList = memberDaysOff.ToList();

            var daysOff = memberDaysOffList.Concat(teamDaysOff).OrderBy(x => x).Distinct().ToList();
            var workDays = iterationWorkDays.Except(daysOff).OrderBy(x => x).Distinct().ToList();

            DaysOff = new ReadOnlyCollection<DateTime>(daysOff);
            MemberDaysOff = new ReadOnlyCollection<DateTime>(memberDaysOffList);
            WorkDays = new ReadOnlyCollection<DateTime>(workDays);

            var memberCapacitySum = memberCapacityPerDay.Sum();

            DailyHourCount = memberCapacitySum.Clamp(0, FullCapacityDailyHourCount);

            HoursTotalCount = DailyHourCount * WorkDays.Count;

            DailyPercent = (DailyHourCount > 0 ? DailyHourCount / FullCapacityDailyHourCount * 100 : 0).Clamp(0, 100);

            TotalWorkDayCount = DailyPercent / 100 * WorkDays.Count;

            WorkDayPercent =
                (workDays.Count > 0 ? workDays.Count / (double)teamWorkDays.Count() * 100 : 0).Clamp(0, 100);
        }

        public double DailyHourCount { get; }

        public double DailyPercent { get; }

        public IReadOnlyCollection<DateTime> DaysOff { get; }

        public bool HasAnyActivities { get; }

        public double HoursTotalCount { get; }

        public IReadOnlyCollection<DateTime> MemberDaysOff { get; }

        public Guid MemberId { get; set; }

        public double TotalWorkDayCount { get; }

        public double WorkDayPercent { get; }

        public IReadOnlyCollection<DateTime> WorkDays { get; }

        public static TeamMemberCapacity Default(Guid memberId, TeamCapacity teamCapacity)
        {
            if (teamCapacity == null) throw new ArgumentNullException(nameof(teamCapacity));

            return new TeamMemberCapacity(
                memberId,
                teamCapacity.IterationWorkDays,
                teamCapacity.TeamDaysOff,
                teamCapacity.WorkDays);
        }

        private static IEnumerable<double> GetMemberCapacityPerDay(IterationCapacityApiResponse capacity)
        {
            return capacity?.Activities?.Select(x => x.CapacityPerDay).ToList() ?? Enumerable.Empty<double>();
        }

        private static IEnumerable<DateTime> GetMemberDaysOff(IterationCapacityApiResponse capacity)
        {
            return capacity?.DaysOff?.SelectMany(x => x.Start.GetWorkDatesUntil(x.End)).ToList()
                   ?? Enumerable.Empty<DateTime>();
        }
    }
}