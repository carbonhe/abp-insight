using System.Collections.Generic;
using AbpInsight.Daemon.Stages.Highlightings;
using AbpInsight.Framework;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Stages.HighlightingStage;

// [DaemonStage(
//     StagesBefore = [typeof(SolutionAnalysisFileStructureCollectorStage)],
//     GlobalAnalysisStage = true,
//     OverridenStages = [typeof(HighlightingStage)])]
public class GlobalHighlightingStage(
    AbpInsighter abpInsighter,
    IEnumerable<IAbpHighlightingProvider> highlightingProviders) : AbstractHighlightingStage(abpInsighter, highlightingProviders);