using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.TeamMeta
{
    public class TeamMetaIteration
    {
        public TeamMetaIteration(
            IterationApiResponseBase iteration, 
            IterationCapacityListApiResponse capacities, 
            IterationDaysOffApiResponse teamDaysOff)
        {
            if (iteration == null) throw new ArgumentNullException(nameof(iteration));
            if (capacities == null) throw new ArgumentNullException(nameof(capacities));
            if (teamDaysOff == null) throw new ArgumentNullException(nameof(teamDaysOff));

            Id = iteration.Id;
            Name = iteration.Name;
            StartDate = iteration.Attributes?.StartDate;
            FinishDate = iteration.Attributes?.FinishDate;
            Url = iteration.Url;

            var capacityList = capacities.Value.Select(x => new Capacity(x)).ToList();
            var teamDaysOffList = teamDaysOff.DaysOff.Select(x => new DayOff(x)).ToList();

            Capacities = new ReadOnlyCollection<Capacity>(capacityList);
            TeamDaysOff = new ReadOnlyCollection<DayOff>(teamDaysOffList);
        }

        public IReadOnlyCollection<Capacity> Capacities { get; }

        public DateTime? FinishDate { get; }

        public string Name { get; }

        public Guid Id { get; }

        public DateTime? StartDate { get; }

        public IReadOnlyCollection<DayOff> TeamDaysOff { get; }

        public string Url { get; }

        public class DayOff
        {
            public DayOff(IterationDayOff dayOff)
            {
                if (dayOff == null) throw new ArgumentNullException(nameof(dayOff));

                Start = dayOff.Start;
                End = dayOff.End;
            }

            public DateTime Start { get;  }

            public DateTime End { get;  }
        }

        public class Capacity
        {
            public Capacity(IterationCapacityApiResponse capacity)
            {
                if (capacity == null) throw new ArgumentNullException(nameof(capacity));

                var activities = capacity.Activities.Select(x => new Activity(x)).ToList();
                var daysOff = capacity.DaysOff.Select(x => new DayOff(x)).ToList();

                Activities = new ReadOnlyCollection<Activity>(activities);
                DaysOff = new ReadOnlyCollection<DayOff>(daysOff);
            }

            public IReadOnlyCollection<Activity> Activities { get; }

            public IReadOnlyCollection<DayOff> DaysOff { get; }

            public class Activity
            {
                public Activity(IterationCapacityApiResponse.Activity activity)
                {
                    if (activity == null) throw new ArgumentNullException(nameof(activity));

                    CapacityPerDay = activity.CapacityPerDay;
                    Name = activity.Name;
                }

                public double CapacityPerDay { get; }

                public string Name { get; }
            }
        }
    }
}