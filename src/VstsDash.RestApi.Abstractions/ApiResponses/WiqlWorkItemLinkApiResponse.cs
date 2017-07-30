using System.Collections.Generic;

namespace VstsDash.RestApi.ApiResponses
{
    public class WiqlWorkItemLinkApiResponse
    {
        public ICollection<WorkItemRelation> WorkItemRelations { get; set; } = new List<WorkItemRelation>();

        public class WorkItemRelation
        {
            public string Rel { get; set; }

            public WorkItemRelationItem Source { get; set; }

            public WorkItemRelationItem Target { get; set; }

            public class WorkItemRelationItem
            {
                public long Id { get; set; }

                public string Url { get; set; }
            }
        }
    }
}