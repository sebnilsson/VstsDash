using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class TeamCapacity
    {
        public TeamCapacity(
            IterationApiResponse iteration,
            IterationDaysOffApiResponse iterationDaysOff,
            TeamMemberListApiResponse teamMembers = null,
            IterationCapacityListApiResponse iterationCapacities = null)
            : this(iteration, iterationDaysOff?.DaysOff, teamMembers, iterationCapacities)
        {
        }

        public TeamCapacity(
            IterationApiResponse iteration,
            IEnumerable<IterationDayOff> iterationDaysOff = null,
            TeamMemberListApiResponse teamMembers = null,
            IterationCapacityListApiResponse iterationCapacities = null)
        {
            if (iteration == null) throw new ArgumentNullException(nameof(iteration));

            var iterationWorkDays = GetIterationWorkDays(iteration).OrderBy(x => x).Distinct().ToList();

            var teamDaysOff = GetTeamDaysOff(iterationDaysOff).OrderBy(x => x).Distinct().ToList();

            var workDays = iterationWorkDays.Except(teamDaysOff).OrderBy(x => x).Distinct().ToList();

            var members = GetMembers(teamMembers?.Value, iterationCapacities?.Value, iterationWorkDays, teamDaysOff)
                .ToList();

            IterationWorkDays = new ReadOnlyCollection<DateTime>(iterationWorkDays);
            Members = new ReadOnlyCollection<TeamMemberCapacity>(members);
            TeamDaysOff = new ReadOnlyCollection<DateTime>(teamDaysOff);
            WorkDays = new ReadOnlyCollection<DateTime>(workDays);
        }

        public double DailyHourCount => Members.Any() ? Members.Average(x => x.DailyHourCount) : 0;

        public double DailyPercent => Members.Any() ? Members.Average(x => x.DailyPercent) : 0;

        public double HoursTotalCount => Members.Any() ? Members.Sum(x => x.HoursTotalCount) : 0;

        public IReadOnlyCollection<DateTime> IterationWorkDays { get; }

        public IReadOnlyCollection<TeamMemberCapacity> Members { get; }

        public IReadOnlyCollection<DateTime> TeamDaysOff { get; }

        public IReadOnlyCollection<DateTime> WorkDays { get; }

        public double TotalWorkDayCount => Members.Any() ? Members.Sum(x => x.HoursTotalCount) : 0;

        private static IEnumerable<TeamMemberCapacity> GetMembers(
            IEnumerable<TeamMemberApiResponse> teamMembers,
            IEnumerable<IterationCapacityApiResponse> capacities,
            IEnumerable<DateTime> iterationWorkDays,
            IEnumerable<DateTime> teamDaysOff)
        {
            teamMembers = teamMembers ?? Enumerable.Empty<TeamMemberApiResponse>();
            capacities = capacities ?? Enumerable.Empty<IterationCapacityApiResponse>();

            return from member in teamMembers
                join c in capacities on member.Id equals c.TeamMember.Id into c
                from capacity in c.DefaultIfEmpty()
                select new TeamMemberCapacity(member.Id, capacity, iterationWorkDays, teamDaysOff);
        }

        private static IEnumerable<DateTime> GetIterationWorkDays(IterationApiResponseBase iteration)
        {
            var iterationStartAt = iteration.Attributes?.StartDate ?? DateTime.MinValue;
            var iterationEndAt = iteration.Attributes?.FinishDate ?? DateTime.MinValue;

            return iterationStartAt != DateTime.MinValue && iterationEndAt != DateTime.MinValue
                ? iterationStartAt.GetWorkDatesUntil(iterationEndAt)
                : Enumerable.Empty<DateTime>();
        }

        private static IEnumerable<DateTime> GetTeamDaysOff(IEnumerable<IterationDayOff> daysOff)
        {
            return daysOff?.SelectMany(x => x.Start.GetWorkDatesUntil(x.End))
                   ?? Enumerable.Empty<DateTime>();
        }
    }
}