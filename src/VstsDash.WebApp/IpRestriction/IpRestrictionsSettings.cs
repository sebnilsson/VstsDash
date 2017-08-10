using System.Collections.Generic;

namespace VstsDash.WebApp.IpRestriction
{
    public class IpRestrictionsSettings
    {
        public const string ConfigurationSectionKey = "IpRestrictions";

        public bool Enable { get; set; }

        public IReadOnlyCollection<string> IpWhiteList { get; set; }
    }
}