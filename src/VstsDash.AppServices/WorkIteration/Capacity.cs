using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class Capacity
    {
        public Capacity(
            TeamMemberApiResponse teamMember,
            IEnumerable<IterationCapacityApiResponse.Activity> activities,
            IEnumerable<IterationDayOff> daysOff)
        {
            TeamMemberId = teamMember.Id;
            TeamMemberDisplayName = teamMember.DisplayName;
            TeamMemberUniqueName = teamMember.UniqueName;

            Activities = activities.ToList();
            DaysOff = daysOff.ToList();
        }

        public string TeamMemberDisplayName { get; set; }

        public Guid TeamMemberId { get; set; }

        public string TeamMemberUniqueName { get; set; }

        public TeamMemberApiResponse TeamMember { get; set; }

        public IReadOnlyCollection<IterationCapacityApiResponse.Activity> Activities { get; set; }

        public IReadOnlyCollection<IterationDayOff> DaysOff { get; set; }

        // TODO: Include team's days off
        public double TotalDaysOff => GetTotalDaysOff();

        public double TotalCapacityPerDay => Activities?.Sum(x => x.CapacityPerDay) ?? 0;

        private double GetTotalDaysOff()
        {
            double daysOff = 0;

            if (DaysOff != null)
                foreach (var dayOff in DaysOff)
                {
                    var differenceTotalDays = dayOff.Start.GetWorkDatesUntil(dayOff.End).Count();
                    daysOff += differenceTotalDays;
                }

            return daysOff;
        }
    }
}