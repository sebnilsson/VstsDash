using System;
using VstsDash.RestApi.ApiResponses;

namespace VstsDash.AppServices.WorkActivity
{
    public class Author
    {
        public Author(TeamMemberApiResponse member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            MemberId = member.Id;
            MemberDisplayName = member.DisplayName;
            MemberImageUrl = member.ImageUrl;
            MemberUniqueName = member.UniqueName;
        }
        
        public Guid MemberId { get; }

        public string MemberDisplayName { get; }

        public string MemberImageUrl { get; }

        public string MemberUniqueName { get; }
    }
}