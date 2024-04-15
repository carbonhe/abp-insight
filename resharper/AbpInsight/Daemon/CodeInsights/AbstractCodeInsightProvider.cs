using System;
using System.Collections.Generic;
using System.Linq;
using AbpInsight.Daemon.Stages.Highlightings;
using AbpInsight.Utils;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.Application.UI.Controls.GotoByName;
using JetBrains.Application.UI.DataContext;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Application.UI.Tooltips;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features.CodeInsights.Providers;
using JetBrains.RdBackend.Common.Features.Services;
using JetBrains.ReSharper.Daemon.CodeInsights;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Model;

namespace AbpInsight.Daemon.CodeInsights;

public abstract class AbstractCodeInsightProvider(
    BulbMenuComponent bulbMenuComponent,
    OccurrencePopupMenu occurrencePopupMenu) : IAbpHighlightingProvider, ICodeInsightsProvider
{
    public virtual string ProviderId => "Abp implicit usage";
    public virtual string DisplayName => "Abp implicit usage";
    public virtual CodeVisionAnchorKind DefaultAnchor => CodeVisionAnchorKind.Top;

    public virtual ICollection<CodeVisionRelativeOrdering> RelativeOrderings { get; } =
        [new CodeVisionRelativeOrderingBefore(ReferencesCodeInsightsProvider.Id)];

    public bool IsAvailableIn(ISolution solution) => true;

    public abstract bool AddHighlighting(ICSharpDeclaration declaration, IHighlightingConsumer consumer);

    public void OnClick(CodeInsightHighlightInfo highlightInfo, ISolution solution)
    {
        if (highlightInfo.CodeInsightsHighlighting is AbpCodeInsightHighlighting highlighting)
        {
            Lifetime.Using(lifetime =>
            {
                var windowContextSource =
                    new PopupWindowContextSource(lt =>
                        new RiderEditorOffsetPopupWindowContext(highlightInfo.CodeInsightsHighlighting.Range.StartOffset.Offset));

                var rules = DataRules.AddRule(nameof(windowContextSource), UIDataConstants.PopupWindowContextSource, windowContextSource);

                var dataContexts = Shell.Instance.GetComponent<DataContexts>();

                var dataContext = dataContexts.CreateOnActiveControl(lifetime, rules);

                var actions = highlighting.ActionFactory.Invoke(dataContext).ToArray();

                if (actions.Any())
                    bulbMenuComponent.ShowBulbMenu(actions, windowContextSource);
            });
        }
    }

    public virtual void OnExtraActionClick(CodeInsightHighlightInfo highlightInfo, string actionId, ISolution solution)
    {
    }

    protected virtual void AddHighlighting(
        IHighlightingConsumer consumer,
        ICSharpDeclaration element,
        IDeclaredElement declaredElement,
        string displayName,
        string tooltip,
        string moreText,
        IconModel iconModel,
        Func<IDataContext, IEnumerable<BulbMenuItem>> actionFactory,
        List<CodeVisionEntryExtraActionModel>? extraActions)
    {
        consumer.AddHighlighting(new AbpCodeInsightHighlighting(
            element.GetNameDocumentRange(),
            displayName,
            tooltip,
            moreText,
            this,
            declaredElement,
            iconModel,
            actionFactory,
            extraActions));
    }

    protected virtual void ShowOccurrences(IDataContext context, string emptyTooltip, InlineSearchRequest inlineSearchRequest)
    {
        var tooltipManager = Shell.Instance.GetComponent<ITooltipManager>();

        Lifetime.Using(lifetime =>
        {
            var occurrences = inlineSearchRequest.Search();


            if (occurrences is not { Count: > 0 })
                tooltipManager.ShowIfPopupWindowContext(emptyTooltip, context);
            else
                occurrencePopupMenu.ShowMenuFromTextControl(
                    context,
                    occurrences,
                    new OccurrencePopupMenuOptions(
                        inlineSearchRequest.Title,
                        true,
                        new OccurrencePresentationOptions
                        {
                            TextDisplayStyle = TextDisplayStyle.ContainingType,
                            LocationStyle = GlobalLocationStyle.None
                        }, null, () => inlineSearchRequest.CreateSearchDescriptor(occurrences)));
        });
    }
}