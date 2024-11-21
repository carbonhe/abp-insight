using System.Collections.Generic;
using AbpInsight.Daemon.Stages.Highlightings;
using AbpInsight.Daemon.Stages.Highlightings.Providers;
using AbpInsight.VoloAbp;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Stages.HighlightingStage;

// [DaemonStage(
//     StagesBefore = [typeof(SolutionAnalysisFileStructureCollectorStage)],
//     GlobalAnalysisStage = true,
//     OverridenStages = [typeof(HighlightingStage)])]
public class GlobalHighlightingStage(
    AbpInsighter abpInsighter,
    IEnumerable<IAbpDeclarationHighlightingProvider> highlightingProviders) : AbstractHighlightingStage(abpInsighter, highlightingProviders);