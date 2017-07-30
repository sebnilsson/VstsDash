using System.Collections.Generic;

namespace VstsDash.RestApi.ApiResponses
{
    public class IterationCapacityApiResponse
    {
        public TeamMemberApiResponse TeamMember { get; set; }

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

        public ICollection<IterationDayOff> DaysOff { get; set; } = new List<IterationDayOff>();

        public string Url { get; set; }

        public class Activity
        {
            public double CapacityPerDay { get; set; }

            public string Name { get; set; }
        }
    }
}