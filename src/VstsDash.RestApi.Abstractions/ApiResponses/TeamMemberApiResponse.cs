using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class TeamMemberApiResponse
    {
        public string DisplayName { get; set; }

        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public string UniqueName { get; set; }

        public string Url { get; set; }
    }
}