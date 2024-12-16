using System.Linq;
using AbpInsight.Services.InlayHints;
using AbpInsight.VoloAbp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace AbpInsight.Daemon.Stages.Analysis;

[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(AbpInsightInlayHighlighting)
    ])]
public class PublishEventInvocationAnalyzer : AbpInsightProblemAnalyzer<IInvocationExpression>
{
    public const string MethodName = "PublishAsync";

    protected override void Analyze(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        var typeElement = TypeFactory.CreateTypeByCLRName(KnownTypes.IEventBus, element.PsiModule).GetTypeElement();

        if (element.InvokedExpression is not IReferenceExpression expression)
            return;

        if (expression.NameIdentifier.Name != MethodName)
            return;

        var (declaredElement, substitution, resolveErrorType) = element.InvocationExpressionReference.Resolve();

        if (resolveErrorType != ResolveErrorType.OK
            || declaredElement is not IMethod { ShortName: MethodName } method
            || method.Parameters.Count == 0 ||
            !Equals(method.ContainingType, typeElement))
            return;

        consumer.AddHighlighting(new AbpInsightInlayHighlighting(element, element.LPar.GetDocumentEndOffset(), null, null, "Abp event dispatch"));
    }
}