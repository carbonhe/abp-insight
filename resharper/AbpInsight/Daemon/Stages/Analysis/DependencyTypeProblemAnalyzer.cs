using System.Linq;
using AbpInsight.Daemon.Errors;
using AbpInsight.Utils;
using AbpInsight.VoloAbp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AbpInsight.Daemon.Stages.Analysis;

[ElementProblemAnalyzer(
    typeof(IClassDeclaration),
    HighlightingTypes =
    [
        typeof(DependencyTypeMustBePublicError),
        typeof(DependencyImplementsMultipleLifetimesWarning)
    ])]
public class DependencyTypeProblemAnalyzer : AbpInsightProblemAnalyzer<IClassDeclaration>
{
    protected override void Analyze(IClassDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        var clazz = element.DeclaredElement;

        if (clazz == null)
            return;

        var attributed = clazz.HasAttributeInstance(KnownTypes.DependencyAttribute, AttributesSource.Self | AttributesSource.Inherited);

        var transient = clazz.DerivesFrom(KnownTypes.ITransientDependency);
        var singleton = clazz.DerivesFrom(KnownTypes.ISingletonDependency);
        var scoped = clazz.DerivesFrom(KnownTypes.IScopedDependency);

        if (!attributed && !transient && !singleton && !scoped)
            return;


        if (element.GetAccessRights() != AccessRights.PUBLIC)
            consumer.AddHighlighting(new DependencyTypeMustBePublicError(element));

        if (new[] { transient, singleton, scoped }.Count(it => it) > 1)
            consumer.AddHighlighting(new DependencyImplementsMultipleLifetimesWarning(element));
    }
}