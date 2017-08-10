using Microsoft.AspNetCore.Razor.TagHelpers;
using VstsDash.AppServices.WorkIteration;

namespace VstsDash.WebApp.TagHelpers
{
    [HtmlTargetElement("state-badge")]
    public class StateBadgeTagHelper : TagHelper
    {
        [HtmlAttributeName("state")]
        public string State { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var isStateDone = WorkItemState.IsDone(State);
            var isStateCommit = WorkItemState.IsCommited(State);
            var isStateInProgress = WorkItemState.IsInProgress(State);

            var stateClass = isStateDone
                                 ? "badge-success"
                                 : (isStateCommit || isStateInProgress ? "badge-primary" : "badge-default");

            var htmlContent = $@"
<small>
    <span class=""badge badge-pill circle {stateClass}"">&nbsp;</span>
</small>
<code>{State}</code>";

            output.Content.SetHtmlContent(htmlContent);

            output.Attributes.RemoveAll(nameof(State));
        }
    }
}