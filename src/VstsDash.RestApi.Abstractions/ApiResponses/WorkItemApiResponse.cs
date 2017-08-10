using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace VstsDash.RestApi.ApiResponses
{
    public class WorkItemApiResponse
    {
        public WorkItemFields Fields { get; set; }

        public long Id { get; set; }

        public ICollection<Relation> Relations { get; set; } = new List<Relation>();

        public long Rev { get; set; }

        public string Url { get; set; }

        public class Relation
        {
            public const string RelHierarchyForward = "System.LinkTypes.Hierarchy-Forward";

            public const string RelHierarchyReverse = "System.LinkTypes.Hierarchy-Reverse";

            public bool IsRelHierarchyForward => Rel == RelHierarchyForward;

            public bool IsRelHierarchyReverse => Rel == RelHierarchyReverse;

            public string Rel { get; set; }

            public string Url { get; set; }

            public int UrlId => GetUrlId();

            private int GetUrlId()
            {
                var lastSlashIndex = Url?.LastIndexOf('/') ?? -1;
                if (lastSlashIndex < 0) return 0;

                var value = Url?.Substring(lastSlashIndex + 1);

                int id;
                int.TryParse(value ?? string.Empty, out id);

                return id;
            }
        }

        public class WorkItemFields
        {
            [JsonProperty("System.AreaPath")]
            public string AreaPath { get; set; }

            [JsonProperty("System.AssignedTo")]
            public string AssignedTo { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.BacklogPriority")]
            public double BacklogPriority { get; set; }

            [JsonProperty("System.BoardColumn")]
            public string BoardColumn { get; set; }

            [JsonProperty("System.ChangedBy")]
            public string ChangedBy { get; set; }

            [JsonProperty("System.ChangedDate")]
            public DateTime? ChangedDate { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.ClosedDate")]
            public DateTime? ClosedDate { get; set; }

            [JsonProperty("System.CreatedBy")]
            public string CreatedBy { get; set; }

            [JsonProperty("System.CreatedDate")]
            public DateTime? CreatedDate { get; set; }

            [JsonProperty("System.Description")]
            public string Description { get; set; }

            [JsonProperty("Microsoft.VSTS.Scheduling.Effort")]
            public double Effort { get; set; }

            [JsonProperty("System.IterationPath")]
            public string IterationPath { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.Priority")]
            public int Priority { get; set; }

            [JsonProperty("System.Reason")]
            public string Reason { get; set; }

            [JsonProperty("Microsoft.VSTS.Scheduling.RemainingWork")]
            public double RemainingWork { get; set; }

            [JsonProperty("System.State")]
            public string State { get; set; }

            public IEnumerable<string> TagList
            {
                get
                {
                    return Tags?.Split(';').Select(x => x?.Trim()).Where(x => !string.IsNullOrWhiteSpace(x))
                           ?? Enumerable.Empty<string>();
                }
            }

            [JsonProperty("System.Tags")]
            public string Tags { get; set; }

            [JsonProperty("System.TeamProject")]
            public string TeamProject { get; set; }

            [JsonProperty("System.Title")]
            public string Title { get; set; }

            [JsonProperty("Microsoft.VSTS.Common.ValueArea")]
            public string ValueArea { get; set; }

            [JsonProperty("System.WorkItemType")]
            public string WorkItemType { get; set; }
        }
    }
}