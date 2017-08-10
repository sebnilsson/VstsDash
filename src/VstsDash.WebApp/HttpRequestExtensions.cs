using System;
using Microsoft.AspNetCore.Http;

namespace VstsDash.WebApp
{
    public static class HttpRequestExtensions
    {
        public static bool IsDashboard(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var isDashboard = (request.Query?["dashboard"] ?? string.Empty) == "1";
            return isDashboard;
        }
    }
}