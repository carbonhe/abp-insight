using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AbpInsight.Daemon.Stages.Highlightings;

public interface IAbpHighlightingProvider
{
    bool AddHighlighting(ICSharpDeclaration declaration, IHighlightingConsumer consumer);
}