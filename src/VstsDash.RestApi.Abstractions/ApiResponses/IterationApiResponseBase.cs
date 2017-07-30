using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class IterationApiResponseBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IterationApiResponseAttributes Attributes { get; set; }

        public string Url { get; set; }

        public class IterationApiResponseAttributes
        {
            public DateTime? FinishDate { get; set; }

            public DateTime? StartDate { get; set; }
        }
    }

}