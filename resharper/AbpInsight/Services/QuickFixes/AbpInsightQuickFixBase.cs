using System.Collections.Generic;
using AbpInsight.Daemon.Stages.Highlightings;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace AbpInsight.Services.QuickFixes;

public abstract class AbpInsightQuickFixBase : QuickFixBase
{
    public override IEnumerable<IntentionAction> CreateBulbItems()
    {
        return this.ToQuickFixIntentions(AbpInsightAnchors.BulbGroup);
    }
}