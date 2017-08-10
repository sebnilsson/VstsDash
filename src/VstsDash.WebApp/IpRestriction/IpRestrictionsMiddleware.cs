using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace VstsDash.WebApp.IpRestriction
{
    public class IpRestrictionsMiddleware
    {
        public readonly IReadOnlyCollection<string> IpWhiteList;
        public readonly RequestDelegate Next;

        public IpRestrictionsMiddleware(RequestDelegate next, IOptions<IpRestrictionsSettings> settings)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));

            if (settings == null) throw new ArgumentNullException(nameof(settings));

            IpWhiteList = settings.Value?.IpWhiteList ?? new string[0];
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            var isIpAddressWhiteListed = IpWhiteList.Contains(ipAddress);
            if (!isIpAddressWhiteListed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await Next(context);
        }
    }
}