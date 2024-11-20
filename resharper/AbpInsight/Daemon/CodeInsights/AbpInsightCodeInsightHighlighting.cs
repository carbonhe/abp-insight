using System;
using System.Collections.Generic;
using AbpInsight.Daemon.Stages.Highlightings;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CodeInsights;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.Rider.Model;
using JetBrains.TextControl.DocumentMarkup;
using Severity = JetBrains.ReSharper.Feature.Services.Daemon.Severity;

namespace AbpInsight.Daemon.CodeInsights;

[RegisterHighlighter(Id,
    GroupId = HighlighterGroupIds.HIDDEN,
    Layer = HighlighterLayer.SYNTAX + 1,
    NotRecyclable = true,
    TransmitUpdates = true)]
[StaticSeverityHighlighting(Severity.INFO, typeof(HighlightingGroupIds.CodeInsights), AttributeId = Id, OverlapResolve = OverlapResolveKind.NONE)]
public class AbpInsightCodeInsightHighlighting(
    DocumentRange range,
    string displayText,
    string tooltipText,
    string moreText,
    ICodeInsightsProvider provider,
    IDeclaredElement elt,
    IconModel? icon,
    Func<IDataContext, IEnumerable<BulbMenuItem>> createBulbMenuItems,
    List<CodeVisionEntryExtraActionModel>? extraActions)
    : CodeInsightsHighlighting(range, displayText, tooltipText, moreText, provider, elt, icon, extraActions), IAbpInsightIndicatorHighlighting
{
    private new const string Id = "AbpInsightCodeInsights";

    public IEnumerable<BulbMenuItem> CreateBulbMenuItems(IDataContext dataContext) => createBulbMenuItems(dataContext);
}