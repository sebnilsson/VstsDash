using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class ProjectApiResponse
    {
        public string Description { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string Url { get; set; }
    }
}