using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.RestApi.ApiResponses;
using VstsDash.AppServices.WorkIteration;

namespace VstsDash.AppServices.WorkLeaderboard
{
    public class LeaderboardCapacity : IterationCapacity
    {
        private const int MaxCapacityMultiplier = 4;

        public LeaderboardCapacity(
            IterationApiResponse iteration,
            IEnumerable<DateTime> teamDaysOff,
            IterationCapacityApiResponse memberCapacity = null)
            : base(
                iteration?.Attributes?.StartDate,
                iteration?.Attributes?.FinishDate,
                teamDaysOff,
                memberCapacity?.Activities?.Select(x => x.CapacityPerDay),
                memberCapacity?.DaysOff?.SelectMany(x => x.Start.GetDatesUntil(x.End)))
        {
            if (iteration == null) throw new ArgumentNullException(nameof(iteration));
            if (teamDaysOff == null) throw new ArgumentNullException(nameof(teamDaysOff));

            Multiplier =
                (DailyPercent > 0 ? 100 / DailyPercent : MaxCapacityMultiplier)
                .Clamp(1, MaxCapacityMultiplier);
        }

        public double Multiplier { get; }
    }
}