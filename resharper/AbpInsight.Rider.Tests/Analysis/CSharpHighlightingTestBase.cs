using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;

namespace AbpInsight.Rider.Tests.Analysis;

public abstract class CSharpHighlightingTestBase<T> : CSharpHighlightingTestBase
{
    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile,
        IContextBoundSettingsStore settingsStore)
    {
        return highlighting is T;
    }
}