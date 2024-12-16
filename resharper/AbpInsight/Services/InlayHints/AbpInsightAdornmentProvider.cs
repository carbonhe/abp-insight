using AbpInsight.Resources;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.TextControl.DocumentMarkup.Adornments;

namespace AbpInsight.Services.InlayHints;

[SolutionComponent(Instantiation.DemandAnyThreadSafe)]
public class AbpInsightAdornmentProvider : IHighlighterAdornmentProvider
{
    public bool IsValid(IHighlighter highlighter)
    {
        return highlighter.GetHighlighting() is AbpInsightInlayHighlighting highlighting && highlighting.IsValid();
    }

    public IAdornmentDataModel? CreateDataModel(IHighlighter highlighter)
    {
        if (highlighter.GetHighlighting() is AbpInsightInlayHighlighting highlighting && highlighting.IsValid())
        {
            var data = new AdornmentData().WithText(highlighting.ToolTip).WithIcon(AbpInsightIcons.Logo.Id).WithMode(PushToHintMode.Always);
            return new AdornmentDataModel(data);
        }

        return null;
    }
}