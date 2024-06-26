﻿using System;
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
public class AbpCodeInsightHighlighting(
    DocumentRange range,
    string displayText,
    string tooltipText,
    string moreText,
    ICodeInsightsProvider provider,
    IDeclaredElement elt,
    IconModel? icon,
    Func<IDataContext, IEnumerable<BulbMenuItem>> actionFactory,
    List<CodeVisionEntryExtraActionModel>? extraActions)
    : CodeInsightsHighlighting(range, displayText, tooltipText, moreText, provider, elt, icon, extraActions), IAbpIndicatorHighlighting
{
    private new const string Id = "AbpCodeInsights";

    public Func<IDataContext, IEnumerable<BulbMenuItem>> ActionFactory { get; } = actionFactory;
}