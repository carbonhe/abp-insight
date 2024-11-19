using System.Collections.Generic;
using AbpInsight.Daemon.Stages.Highlightings;
using AbpInsight.Daemon.Stages.Highlightings.Providers;
using AbpInsight.Framework;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Stages.HighlightingStage;

[DaemonStage(StagesBefore = [typeof(CSharpErrorStage), typeof(SmartResolverStage)])]
public class HighlightingStage(
    AbpInsighter abpInsighter,
    IEnumerable<IAbpDeclarationHighlightingProvider> highlightingProviders) : AbstractHighlightingStage(abpInsighter, highlightingProviders);