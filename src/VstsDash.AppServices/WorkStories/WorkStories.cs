using System;
using System.Collections.Generic;
using System.Linq;
using VstsDash.AppServices.WorkIteration;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkStories
{
    public class WorkStories
    {
        public WorkStories(
            DateTime fromDate,
            DateTime toDate,
            TeamCapacity teamCapacity,
            IterationApiResponse iteration,
            Iteration workIteration)
        {
            if (iteration == null)
                throw new ArgumentNullException(nameof(iteration));
            if (workIteration == null)
                throw new ArgumentNullException(nameof(workIteration));

            TeamCapacity = teamCapacity ?? throw new ArgumentNullException(nameof(teamCapacity));

            FromDate = fromDate;
            ToDate = toDate;
            IterationName = iteration.Name;

            Stories = GetStoryItems(workIteration);
        }

        public DateTime FromDate { get; }

        public string IterationName { get; }

        public IReadOnlyCollection<Story> Stories { get; }

        public TeamCapacity TeamCapacity { get; }

        public DateTime ToDate { get; }

        private static IReadOnlyCollection<Story> GetStoryItems(Iteration workIteration)
        {
            var items = workIteration.Items ?? Enumerable.Empty<WorkItem>();

            return items.Where(x => x.IsTypeProductBacklogItem && x.AssignedToMember != null)
                .Select(x => new Story(x))
                .OrderByDescending(x => x.ClosedDate)
                .ThenByDescending(x => x.ChangedDate)
                .ToList();
        }
    }
}