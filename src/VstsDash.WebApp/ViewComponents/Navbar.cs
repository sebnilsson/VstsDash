using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VstsDash.WebApp.ViewComponents
{
    public class Navbar : ViewComponent
    {
        private IEnumerable<KeyValuePair<string, string>> Items => GetItems().ToList();

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var requestUrl = $"{Request.Path}{Request.QueryString}";

            var model = Items.Select(x => (Name: x.Key, Url: x.Value, IsActive: UrlEquals(requestUrl, x.Value)))
                .ToList();

            return await Task.FromResult(View(model));
        }

        private static bool UrlEquals(string url1, string url2)
        {
            return url1 != null && url2 != null && url1.TrimEnd('/')
                       .Equals(url2.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);
        }

        private IEnumerable<KeyValuePair<string, string>> GetItems()
        {
            yield return new KeyValuePair<string, string>("Activity", Url.WorkActivity());
            yield return new KeyValuePair<string, string>("Team board", Url.WorkTeamBoard());
        }
    }
}