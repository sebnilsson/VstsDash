using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class TeamApiResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public string IdentityUrl { get; set; }
    }
}