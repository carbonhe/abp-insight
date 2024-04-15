using AbpInsight.Daemon.Stages.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AbpInsight.Utils;

public static class HighlightingUtils
{
    public static void AddImplicitUsedHighlighting(this IHighlightingConsumer consumer, ICSharpDeclaration declaration)
    {
        consumer.AddHighlighting(new AbpImplicitlyUsedIdentifierHighlighting(declaration.GetNameDocumentRange()));
    }
}