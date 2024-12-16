using AbpInsight.Daemon.Stages.Highlightings;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.InlayHints;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.RichText;

namespace AbpInsight.Services.InlayHints;

[RegisterHighlighter(AttributeId, EffectType = EffectType.INTRA_TEXT_ADORNMENT, Layer = HighlighterLayer.ADDITIONAL_SYNTAX, TransmitUpdates = true)]
[DaemonAdornmentProvider(typeof(AbpInsightAdornmentProvider))]
[DaemonTooltipProvider(typeof(InlayHintTooltipProvider))]
[StaticSeverityHighlighting(Severity.INFO, typeof(HighlightingGroupIds.IntraTextAdornments), AttributeId = AttributeId)]
public class AbpInsightInlayHighlighting(
    ITreeNode node,
    DocumentOffset offset,
    string? toolTip,
    string? errorStripeToolTip,
    RichText? description) : IAbpInsightHighlighting, IInlayHintWithDescriptionHighlighting
{
    public const string AttributeId = "AbpInsightInlayHighlighting";

    public string? ToolTip { get; } = toolTip;

    public string? ErrorStripeToolTip { get; } = errorStripeToolTip;

    public RichText? Description { get; } = description;

    public bool IsValid()
    {
        return node.IsValid();
    }

    public DocumentRange CalculateRange()
    {
        return new DocumentRange(offset);
    }
}