using System.Collections.Generic;
using System.Linq;
using AbpInsight.Daemon.Errors;
using AbpInsight.Services.InlayHints;
using AbpInsight.Utils;
using AbpInsight.VoloAbp;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.InlayHints;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace AbpInsight.Daemon.Stages.Analysis;

[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(PublishAbpEventWarning)
    ])]
public class PublishAbpEventAnalyzer(ISettingsStore settingsStore) : AbpInsightProblemAnalyzer<IInvocationExpression>
{
    private const string MethodName = "PublishAsync";

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
        consumer.AddHighlighting(new PublishAbpEventWarning(element, (element.InvokedExpression as IReferenceExpression)?.Reference, "LocalEventBus"));


        return;

        consumer.AddHighlighting(
            new AbpInsightInlayHighlighting(element, element.LPar.GetDocumentEndOffset(), "Abp event dispatch",
                source => new InlineSearchRequest(
                    "Abp event handlers",
                    element.GetSolution(),
                    new[] { method },
                    pi => SearchEventHandlers(method, element, pi)).ShowOccurrences(element.LPar.GetDocumentRange(), "No related event handlers"),
                [
                    IntraTextAdornmentDataModelHelper.CreateTurnOffAllInlayHintsBulbMenuItem(settingsStore)
                ]));
    }


    private static ICollection<IOccurrence> SearchEventHandlers(IMethod method, IInvocationExpression element, IProgressIndicator pi)
    {
        if (method.Parameters.Count == 2)
        {
            var baseType = TypeFactory.CreateTypeByCLRName(KnownTypes.ILocalEventHandler, element.PsiModule).GetTypeElement()!;
            var tp = baseType.TypeParameters[0];
            var eventDataType = element.Arguments[0].Value?.Type();


            if (eventDataType != null)
            {
                var substitution = EmptySubstitution.INSTANCE.Extend(tp, eventDataType);

                var consumer = new FilterBySubstitutionSearchResultsConsumer(
                    [new DeclaredElementInstance(baseType, substitution)], new SearchResultsConsumer(), true);

                method.GetPsiServices().ParallelFinder.FindInheritors(baseType, consumer, pi);

                var occurrences = consumer.GetOccurrences()
                    .Where(it =>
                    {
                        if (it is DeclaredElementOccurrence o && o.GetDeclaredElement() is IClass clazz)
                        {
                            return !clazz.HasTypeParameters() && !clazz.IsAbstract;
                        }

                        return false;
                    }).ToList();

                return occurrences;
            }
        }

        return new List<IOccurrence>();
    }
}