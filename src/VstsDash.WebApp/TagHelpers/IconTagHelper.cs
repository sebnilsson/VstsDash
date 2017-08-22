using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace VstsDash.WebApp.TagHelpers
{
    [HtmlTargetElement("icon", TagStructure = TagStructure.WithoutEndTag)]
    public class IconTagHelper : TagHelper
    {
        [HtmlAttributeName("class")]
        public string CssClass { get; set; }

        [HtmlAttributeName("is-fixed-width")]
        public bool IsFixedWidth { get; set; } = true;

        [HtmlAttributeName("value")]
        public string Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.TagName = "i";
            output.TagMode = TagMode.StartTagAndEndTag;

            var value = Value.StartsWith("fa-") ? Value : $"fa-{Value}";

            var cssClass = $"fa {value} {(IsFixedWidth ? "fa-fw" : null)} {CssClass}".Trim();
            
            output.Attributes.SetAttribute("class", cssClass);
        }
    }
}