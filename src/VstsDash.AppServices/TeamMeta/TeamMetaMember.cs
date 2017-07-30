using System;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.TeamMeta
{
    public class TeamMetaMember
    {
        public TeamMetaMember(TeamMemberApiResponse member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            Id = member.Id;
            DisplayName = member.DisplayName;
            ImageUrl = member.ImageUrl;
            UniqueName = member.UniqueName;
            Url = member.Url;
        }

        public Guid Id { get; }

        public string DisplayName { get; }

        public string ImageUrl { get; }

        public string UniqueName { get; }

        public string Url { get; }
    }
}