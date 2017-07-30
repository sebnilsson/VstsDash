using System.Collections.Generic;

namespace VstsDash.RestApi.ApiResponses
{
    public class IterationDaysOffApiResponse
    {
        public ICollection<IterationDayOff> DaysOff { get; set; } = new List<IterationDayOff>();

        public string Url { get; set; }
    }
}