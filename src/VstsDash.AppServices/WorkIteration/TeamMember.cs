using System;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkIteration
{
    public class TeamMember
    {
        public TeamMember(TeamMemberApiResponse teamMember)
        {
            if (teamMember == null)
                throw new ArgumentNullException(nameof(teamMember));

            DisplayName = teamMember.DisplayName;
            Id = teamMember.Id;
            ImageUrl = teamMember.ImageUrl;
            UniqueName = teamMember.UniqueName;
        }

        public Guid Id { get; }

        public string DisplayName { get; set; }

        public string ImageUrl { get; set; }

        public string UniqueName { get; set; }
    }
}