using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class IterationCapacity
    {
        private const int FullCapacityDailyHourCount = 8;

        public IterationCapacity(
            IterationApiResponse iteration,
            IterationDaysOffApiResponse teamDaysOff,
            IterationCapacityListApiResponse capacities = null)
            : this(
                iteration?.Attributes?.StartDate,
                iteration?.Attributes?.FinishDate,
                teamDaysOff?.DaysOff.SelectMany(x => x.Start.GetWorkDatesUntil(x.End)),
                capacities?.Value.SelectMany(c => c.Activities.Select(a => a.CapacityPerDay)),
                capacities?.Value.SelectMany(c => c.DaysOff.SelectMany(d => d.Start.GetWorkDatesUntil(d.End))))
        {
        }

        public IterationCapacity(
            DateTime? iterationStartAt = null,
            DateTime? iterationEndAt = null,
            IEnumerable<DateTime> teamDaysOff = null,
            IEnumerable<double> memberCapacityPerDay = null,
            IEnumerable<DateTime> memberDaysOff = null)
        {
            var memberDaysOffList = WorkDatesUtility.GetWorkDates(memberDaysOff ?? Enumerable.Empty<DateTime>())
                .Distinct()
                .ToList();

            var teamDaysOffList = WorkDatesUtility.GetWorkDates(teamDaysOff ?? Enumerable.Empty<DateTime>())
                .Distinct()
                .ToList();

            var iterationWorkDays = GetIterationWorkDays(iterationStartAt, iterationEndAt).ToList();

            var allDaysOff = teamDaysOffList.Concat(memberDaysOffList).Distinct().ToList();
            var netWorkDays = iterationWorkDays.Except(allDaysOff).ToList();

            AllDaysOff = new ReadOnlyCollection<DateTime>(allDaysOff);
            IterationWorkDays = new ReadOnlyCollection<DateTime>(iterationWorkDays);
            MemberDaysOff = new ReadOnlyCollection<DateTime>(memberDaysOffList);
            NetWorkDays = new ReadOnlyCollection<DateTime>(netWorkDays);
            TeamDaysOff = new ReadOnlyCollection<DateTime>(teamDaysOffList);

            var memberCapacitySum = memberCapacityPerDay?.Sum() ?? 0;

            DailyHourCount = memberCapacitySum.Clamp(0, FullCapacityDailyHourCount);

            HoursTotalCount = DailyHourCount * NetWorkDays.Count;

            DailyPercent = (DailyHourCount > 0 ? DailyHourCount / FullCapacityDailyHourCount * 100 : 0)
                .Clamp(0, 100);

            WorkDaysTotalCount = DailyPercent / 100 * NetWorkDays.Count;
        }

        public double DailyHourCount { get; }

        public double DailyPercent { get; }

        public double HoursTotalCount { get; }

        public IReadOnlyCollection<DateTime> AllDaysOff { get; }

        public IReadOnlyCollection<DateTime> IterationWorkDays { get; }

        public IReadOnlyCollection<DateTime> NetWorkDays { get; }

        public IReadOnlyCollection<DateTime> MemberDaysOff { get; }

        public IReadOnlyCollection<DateTime> TeamDaysOff { get; }

        public double WorkDaysTotalCount { get; }

        private static IEnumerable<DateTime> GetIterationWorkDays(DateTime? iterationStartAt, DateTime? iterationEndAt)
        {
            var iterationStart = iterationStartAt ?? DateTime.MinValue;
            var iterationEnd = iterationEndAt ?? DateTime.MinValue;

            return iterationStart != DateTime.MinValue && iterationEnd != DateTime.MinValue
                ? iterationStart.GetWorkDatesUntil(iterationEnd)
                : Enumerable.Empty<DateTime>();
        }
    }
}