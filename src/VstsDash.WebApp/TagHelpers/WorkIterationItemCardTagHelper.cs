using Microsoft.AspNetCore.Razor.TagHelpers;
using VstsDash.AppServices.WorkIteration;

namespace VstsDash.WebApp.TagHelpers
{
    [HtmlTargetElement("work-iteration-item-card")]
    public class WorkIterationItemCardTagHelper : TagHelper
    {
        [HtmlAttributeName("state")]
        public string State { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var cssClass = GetCssClass();

            output.MergeCssAttribute(cssClass);

            output.TagName = "div";

            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.RemoveAll(nameof(State));
        }

        private string GetCssClass()
        {
            var isStateDone = WorkItemState.IsDone(State);
            var isStateCommit = WorkItemState.IsCommited(State);
            var isStateInProgress = WorkItemState.IsInProgress(State);

            var stateClass = isStateDone
                ? "card-outline-success"
                : (isStateCommit || isStateInProgress ? "card-outline-primary" : null);

            var cssClass = $"card work-iteration-item {stateClass}".Trim();
            return cssClass;
        }
    }
}