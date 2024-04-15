using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Stages.Highlightings;

// RegisterConfigurableHighlightingsGroup registers a group in Inspection Severity
[RegisterConfigurableHighlightingsGroup(Abp, "Abp")]
[RegisterConfigurableHighlightingsGroup(AbpPerformance, "Abp Performance Inspections")]
public static class AbpHighlightingGroupIds
{
    public const string Abp = "ABP";
    public const string AbpPerformance = "ABP_PERFORMANCE";
}