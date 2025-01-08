using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Stages.Highlightings;

[ShellComponent(Instantiation.DemandAnyThreadSafe)]
public class AbpInsightHighlightingCustomPresentationsForSeverityProvider : IHighlightingCustomPresentationsForSeverityProvider
{
    public IEnumerable<string> GetAttributeIdsForSeverity(Severity severity)
    {
        if (severity is Severity.HINT or Severity.WARNING)
        {
            return [AbpInsightHighlightingAttributeIds.ABPINSIGHT_PUBLISH_EVENT];
        }

        return [];
    }
}