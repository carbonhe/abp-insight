using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Stages.Highlightings;

// RegisterConfigurableHighlightingsGroup registers a group in Inspection Severity
[RegisterConfigurableHighlightingsGroup(AbpInsight, AbpInsight)]
[RegisterConfigurableHighlightingsGroup(AbpInsightPerformance, "AbpInsight Performance Inspections")]
public static class AbpInsightHighlightingGroupIds
{
    public const string AbpInsight = "AbpInsight";
    public const string AbpInsightPerformance = "AbpInsightPerformance";
}