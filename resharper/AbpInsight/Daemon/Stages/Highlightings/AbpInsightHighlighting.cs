using System.Collections.Generic;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.ReSharper.Feature.Services.Daemon;

namespace AbpInsight.Daemon.Stages.Highlightings;

public interface IAbpInsightHighlighting;

public interface IAbpInsightAnalyzerHighlighting : IAbpInsightHighlighting;

public interface IAbpInsightIndicatorHighlighting : IAbpInsightHighlighting;

public interface IAbpInsightGutterMarkHighlighting : ICustomAttributeIdHighlighting, IAbpInsightIndicatorHighlighting
{
    IEnumerable<BulbMenuItem>? MenuItems { get; }
}

// Add a marker interface to all of our highlights. If we specify a baseClass in ErrorsGen, we have to provide an
// actual class with an abstract IsValid method, because ErrorsGen will declare IsValid as an override.
public abstract class CSharpAbpInsightHighlightingBase : IAbpInsightHighlighting
{
    public abstract bool IsValid();
}