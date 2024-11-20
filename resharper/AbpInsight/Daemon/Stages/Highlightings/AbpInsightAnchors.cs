using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.Application.UI.Controls.BulbMenu.Positions;

namespace AbpInsight.Daemon.Stages.Highlightings;

public static class AbpInsightAnchors
{
    public static readonly IAnchor BulbGroup = new GroupAnchor(BulbMenuAnchors.FirstClassContextItems, AnchorPosition.BeforePosition, "Abp Insight");
}