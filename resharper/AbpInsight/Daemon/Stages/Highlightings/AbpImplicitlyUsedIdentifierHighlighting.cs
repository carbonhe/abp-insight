using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;

namespace AbpInsight.Daemon.Stages.Highlightings;

[StaticSeverityHighlighting(
    Severity.INFO,
    typeof(HighlightingGroupIds.IdentifierHighlightings),
    AttributeId = AbpHighlightingAttributeIds.ImplicitlyUsedIdentifier,
    Languages = CSharpLanguage.Name,
    OverlapResolve = OverlapResolveKind.NONE)]
public class AbpImplicitlyUsedIdentifierHighlighting(DocumentRange documentRange) : IHighlighting, IAbpIndicatorHighlighting
{
    public bool IsValid() => true;

    public DocumentRange CalculateRange() => documentRange;

    public string? ToolTip => null;
    public string? ErrorStripeToolTip => null;
}