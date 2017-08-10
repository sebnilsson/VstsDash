using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace VstsDash.WebApp.IpRestriction
{
    public static class ApplicationBuilderIpRestrictionsExtensions
    {
        public static void UseIpRestrictions(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            var settings = configuration.GetSection(IpRestrictionsSettings.ConfigurationSectionKey)
                .Get<IpRestrictionsSettings>();

            var isEnabled = settings?.Enable ?? false;
            if (!isEnabled) return;

            app.UseMiddleware<IpRestrictionsMiddleware>();
        }
    }
}