using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VstsDash.WebApp.IpRestriction
{
    public static class ServiceCollectionIpRestrictionsExtensions
    {
        public static void AddIpRestrictions(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var ipRestrictionSettingsSection = configuration.GetSection(IpRestrictionsSettings.ConfigurationSectionKey);
            var ipRestrictionSettings = ipRestrictionSettingsSection.Get<IpRestrictionsSettings>();

            var isIpRestrictionsEnabled = ipRestrictionSettings?.Enable ?? false;
            if (!isIpRestrictionsEnabled)
                return;

            services.Configure<IpRestrictionsSettings>(ipRestrictionSettingsSection);
        }
    }
}