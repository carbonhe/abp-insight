using AbpInsight.Daemon.Errors;
using AbpInsight.VoloAbp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace AbpInsight.Daemon.Stages.Analysis;

[ElementProblemAnalyzer(
    typeof(IAttribute),
    HighlightingTypes =
    [
        typeof(DependsOnNonAbpModuleTypeError)
    ])]
public class DependsOnAttributeProblemAnalyzer : AbpInsightProblemAnalyzer<IAttribute>
{
    protected override void Analyze(IAttribute element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.TypeReference?.Resolve().DeclaredElement is not ITypeElement attributeTypeElement)
            return;

        if (!attributeTypeElement.GetClrName().Equals(KnownTypes.DependsOnAttribute))
            return;

        foreach (var argument in element.Arguments)
        {
            if (argument.Expression is ITypeofExpression typeofExpression)
            {
                var argumentType = typeofExpression.ArgumentType;
                if (argumentType.IsSimplePredefined())
                {
                    consumer.AddHighlighting(
                        new DependsOnNonAbpModuleTypeError(typeofExpression.TypeName, argumentType.ToITypeOrUnknown().ToString()));
                }
                else
                {
                    var clazz = argumentType.GetClassType();
                    if (clazz != null && !AbpInsighter.IsAbpModuleType(clazz))
                        consumer.AddHighlighting(new DependsOnNonAbpModuleTypeError(typeofExpression.TypeName, clazz.ShortName));
                }
            }
        }
    }
}