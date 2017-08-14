﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace VstsDash.WebApp.TagHelpers
{
    [HtmlTargetElement("stat", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class StatTagHelper : TagHelper
    {
        private const string DefaultCssClass = "mb-0";

        private const string DefaultValueCss = "font-weight-bold monospace";

        [HtmlAttributeName("class")]
        public string CssClass { get; set; }

        [HtmlAttributeName("description")]
        public string Description { get; set; }

        [HtmlAttributeName("description-css")]
        public string DescriptionCss { get; set; }

        [HtmlAttributeName("is-description-one-line")]
        public bool IsDescriptionOneLine { get; set; } = true;

        [HtmlAttributeName("is-description-small")]
        public bool IsDescriptionSmall { get; set; } = true;

        [HtmlAttributeName("is-inverse")]
        public bool IsInverse { get; set; }

        [HtmlAttributeName("title")]
        public string Title { get; set; }

        [HtmlAttributeName("value")]
        public string Value { get; set; }

        [HtmlAttributeName("value-css")]
        public string ValueCss { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.TagName = "dl";
            output.TagMode = TagMode.StartTagAndEndTag;

            var cssClass = IsInverse
                               ? $"{DefaultCssClass} bg-inverse text-white border-white-1 {CssClass}".Trim()
                               : $"{DefaultCssClass} {CssClass}".Trim();

            output.Attributes.SetAttribute("class", cssClass);

            if (!string.IsNullOrWhiteSpace(Title)) output.Attributes.SetAttribute("title", Title);

            var dt = GetDtTag();
            var dd = await GetDdTag(output);

            output.Content.AppendHtml(dt);
            output.Content.AppendHtml(dd);
        }

        private async Task<TagBuilder> GetDdTag(TagHelperOutput output)
        {
            var description = !string.IsNullOrWhiteSpace(Description)
                                  ? Description
                                  : (output.Content.IsModified
                                         ? output.Content.GetContent()
                                         : (await output.GetChildContentAsync()).GetContent());

            var ddTag = new TagBuilder("dd");
            ddTag.InnerHtml.AppendHtml(description);

            var defaultDescriptionCss =
                $"{(IsDescriptionOneLine ? "one-line" : null)} {(IsDescriptionSmall ? "small" : null)}".Trim();
            var cssClass = $"{defaultDescriptionCss} {DescriptionCss}".Trim();

            ddTag.AddCssClass(cssClass);

            return ddTag;
        }

        private TagBuilder GetDtTag()
        {
            var dtTag = new TagBuilder("dt");
            dtTag.InnerHtml.Append(Value);

            var cssClass = $"{DefaultValueCss} {ValueCss}".Trim();
            dtTag.AddCssClass(cssClass);

            return dtTag;
        }
    }
}