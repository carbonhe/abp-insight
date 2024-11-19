using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Application.Parts;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.Application.UI.Controls.GotoByName;
using JetBrains.Application.UI.DataContext;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features.CodeInsights.Providers;
using JetBrains.RdBackend.Common.Features.Services;
using JetBrains.ReSharper.Daemon.CodeInsights;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Backend.Platform.Icons;
using JetBrains.Rider.Model;
using JetBrains.UI.Icons;

namespace AbpInsight.Daemon.CodeInsights;

[SolutionComponent(Instantiation.DemandAnyThreadUnsafe)]
public class CodeInsightProvider(
    BulbMenuComponent bulbMenuComponent,
    IconHost iconHost) : ICodeInsightsProvider
{
    public virtual string ProviderId => "Abp implicit usage";
    public virtual string DisplayName => "Abp implicit usage";
    public virtual CodeVisionAnchorKind DefaultAnchor => CodeVisionAnchorKind.Top;

    public virtual ICollection<CodeVisionRelativeOrdering> RelativeOrderings { get; } =
        [new CodeVisionRelativeOrderingBefore(ReferencesCodeInsightsProvider.Id)];

    public bool IsAvailableIn(ISolution solution) => true;

    public void OnClick(CodeInsightHighlightInfo highlightInfo, ISolution solution)
    {
        if (highlightInfo.CodeInsightsHighlighting is AbpCodeInsightHighlighting highlighting)
        {
            Lifetime.Using(lt =>
            {
                var windowContextSource = new PopupWindowContextSource(lt =>
                    new RiderEditorOffsetPopupWindowContext(highlightInfo.CodeInsightsHighlighting.Range.StartOffset.Offset));
                var rules = DataRules.AddRule(nameof(windowContextSource), UIDataConstants.PopupWindowContextSource, windowContextSource);
                var dataContexts = Shell.Instance.GetComponent<DataContexts>();
                var dataContext = dataContexts.CreateOnActiveControl(lt, rules);
                var menuItems = highlighting.CreateBulbMenuItems(dataContext).ToList();
                if (menuItems.Any())
                    bulbMenuComponent.ShowBulbMenu(menuItems, windowContextSource);
            });
        }
    }

    public virtual void OnExtraActionClick(CodeInsightHighlightInfo highlightInfo, string actionId, ISolution solution)
    {
    }

    public virtual void AddHighlighting(
        IHighlightingConsumer consumer,
        ICSharpDeclaration element,
        IDeclaredElement declaredElement,
        string displayName,
        string tooltip,
        string moreText,
        IconId iconId,
        Func<IDataContext, IEnumerable<BulbMenuItem>> createMenuItems,
        List<CodeVisionEntryExtraActionModel>? extraActions)
    {
        consumer.AddHighlighting(new AbpCodeInsightHighlighting(
            element.GetNameDocumentRange(),
            displayName,
            tooltip,
            moreText,
            this,
            declaredElement,
            iconHost.Transform(iconId),
            createMenuItems,
            extraActions));
    }
}