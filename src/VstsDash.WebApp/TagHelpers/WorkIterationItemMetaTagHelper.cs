using Microsoft.AspNetCore.Razor.TagHelpers;

namespace VstsDash.WebApp.TagHelpers
{
    [HtmlTargetElement("work-iteration-item-meta")]
    public class WorkIterationItemMetaTagHelper : TagHelper
    {
        private const string ChildCssClass = "col-xl-6 col-lg-4 col-md-6 col-sm-6";

        private const string CssClass = "col-xl-3 col-lg-4 col-md-6";

        private const string WideChildCssClass = "col-xl-6 col-lg-4 col-md-6 col-sm-6";

        private const string WideCssClass = "col-lg-6 col-md-6 col-sm-12";

        [HtmlAttributeName("is-child")]
        public bool IsChild { get; set; }

        [HtmlAttributeName("is-wide")]
        public bool IsWide { get; set; }

        [HtmlAttributeName("label")]
        public string Label { get; set; }

        [HtmlAttributeName("value")]
        public string Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrWhiteSpace(Label))
            {
                var labelHtmlContent = $"<span class=\"text-muted\">{Label}</span>: ";
                output.PreContent.AppendHtml(labelHtmlContent);
            }

            if (!string.IsNullOrWhiteSpace(Value))
            {
                var valueHtmlContent = $"<code>{Value}</code> ";
                output.PreContent.AppendHtml(valueHtmlContent);
            }

            var cssClass = IsChild && IsWide
                ? WideChildCssClass
                : (IsChild ? ChildCssClass : (IsWide ? WideCssClass : CssClass));

            output.TagName = "div";

            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("class", $"work-iteration-item-meta {cssClass}");

            output.Attributes.RemoveAll("is-child");
            output.Attributes.RemoveAll("is-wide");
            output.Attributes.RemoveAll(nameof(Label));
            output.Attributes.RemoveAll(nameof(Value));
        }

        private string GetHtmlContent()
        {
            if (string.IsNullOrWhiteSpace(Value))
                return null;

            var htmlContent = !string.IsNullOrWhiteSpace(Label) ? $"{Label}: " : null;

            htmlContent += $"<code>{Value}</code>";

            return htmlContent;
        }
    }
}