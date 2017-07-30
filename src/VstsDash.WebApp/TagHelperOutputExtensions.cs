using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace VstsDash.WebApp
{
    public static class TagHelperOutputExtensions
    {
        public static void MergeCssAttribute(this TagHelperOutput tagHelperOutput, string cssClass)
        {
            if (tagHelperOutput == null)
                throw new ArgumentNullException(nameof(tagHelperOutput));

            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass(cssClass);

            tagHelperOutput.MergeAttributes(tagBuilder);
        }
    }
}