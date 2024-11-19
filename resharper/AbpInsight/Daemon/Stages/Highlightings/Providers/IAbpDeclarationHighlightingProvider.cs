using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace AbpInsight.Daemon.Stages.Highlightings.Providers;

public interface IAbpDeclarationHighlightingProvider
{
    bool AddDeclarationHighlighting(IDeclaration treeNode, IHighlightingConsumer consumer);
}