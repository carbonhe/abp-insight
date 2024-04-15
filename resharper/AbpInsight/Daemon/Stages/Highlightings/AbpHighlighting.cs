namespace AbpInsight.Daemon.Stages.Highlightings;

public interface IAbpHighlighting;

public interface IAbpAnalyzerHighlighting : IAbpHighlighting;

public interface IAbpIndicatorHighlighting : IAbpHighlighting;

// Add a marker interface to all of our highlights. If we specify a baseClass in ErrorsGen, we have to provide an
// actual class with an abstract IsValid method, because ErrorsGen will declare IsValid as an override.
public abstract class CSharpAbpHighlightingBase : IAbpHighlighting
{
    public abstract bool IsValid();
}