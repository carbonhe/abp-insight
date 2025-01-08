using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;

namespace AbpInsight.Daemon.Stages.Highlightings;

[StaticSeverityHighlighting(
    Severity.INFO,
    typeof(HighlightingGroupIds.IdentifierHighlightings),
    AttributeId = AbpInsightHighlightingAttributeIds.ABPINSIGHT_IMPLICITLY_USED_IDENTIFIER,
    Languages = CSharpLanguage.Name,
    OverlapResolve = OverlapResolveKind.NONE)]
public class AbpInsightImplicitlyUsedIdentifierHighlighting(DocumentRange documentRange) : IHighlighting, IAbpInsightIndicatorHighlighting
{
    public bool IsValid() => true;

    public DocumentRange CalculateRange() => documentRange;

    public string? ToolTip => null;
    public string? ErrorStripeToolTip => null;
}