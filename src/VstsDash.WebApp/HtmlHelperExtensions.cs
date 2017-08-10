using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace VstsDash.WebApp
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent Icon(this IHtmlHelper htmlHelper, string icon, string extraAttributes = null)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));
            if (icon == null) throw new ArgumentNullException(nameof(icon));

            var tag = new TagBuilder("i");

            tag.AddCssClass("fa");
            tag.AddCssClass(icon);

            if (!string.IsNullOrWhiteSpace(extraAttributes)) tag.AddCssClass(extraAttributes);

            return tag;
        }

        public static IHtmlContent PartialContent(
            this IHtmlHelper htmlHelper,
            string partialViewName,
            object model,
            object additionalViewData)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));
            if (partialViewName == null) throw new ArgumentNullException(nameof(partialViewName));

            var viewData = new ViewDataDictionary(htmlHelper.ViewData);

            var additionals = HtmlHelper.ObjectToDictionary(additionalViewData);
            foreach (var additional in additionals) viewData[additional.Key] = additional.Value;

            return htmlHelper.Partial(partialViewName, model, viewData);
        }
    }
}