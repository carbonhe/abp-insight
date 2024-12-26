using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Handlers;
using JetBrains.Application.UI.Controls;
using JetBrains.Application.UI.DataContext;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Application.UI.Tooltips;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features.Services;
using JetBrains.ReSharper.Daemon.Specific.InheritedGutterMark;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Feature.Services.Tree.SectionsManagement;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.Coords;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.Icons;

namespace AbpInsight.Utils;

public class InlineSearchRequest(
    string title,
    ISolution solution,
    ICollection searchTargets,
    Func<IProgressIndicator, ICollection<IOccurrence>> search) : SearchRequest
{
    public override string Title => title;

    public override ISolution Solution => solution;

    public override ICollection SearchTargets => searchTargets;

    public override ICollection<IOccurrence> Search(IProgressIndicator progressIndicator) => search(progressIndicator);

    public SearchDescriptor CreateSearchDescriptor(ICollection<IOccurrence>? occurrences = null) => new InlineSearchDescriptor(this, occurrences);


    public void ShowOccurrences(PopupWindowContextSource source, PopupWindowContextSource emptyTooltipSource, string emptyTooltip, IconId? iconId = null)
    {
        var requirementsManager = Shell.Instance.GetComponent<RequirementsManager>();
        var dataContexts = Shell.Instance.GetComponent<DataContexts>();
        var popupMenuBehaviour = Shell.Instance.GetComponent<GutterMarkPopupBehaviour>();
        using var ld = Lifetime.Define();
        var onActiveControl1 = dataContexts.CreateOnActiveControl(ld.Lifetime);
        var textControlPos = onActiveControl1.GetData<ITextControlPos>(TextControlDataConstants.TextControlPosition);
        var instance = CommitAllDocumentsRequirement.TryGetInstance(onActiveControl1);
        requirementsManager.ExecuteActionAsync(instance, (Action)(() =>
        {
            Lifetime.Using(lifetime =>
            {
                var dataRules = DataRules.AddRule<PopupWindowContextSource>("WindowSource", UIDataConstants.PopupWindowContextSource, source);
                var ctx = dataContexts.CreateOnActiveControl(lifetime, dataRules);
                if (textControlPos == null ||
                    !textControlPos.Equals(ctx.GetData<ITextControlPos>(TextControlDataConstants.TextControlPosition)))
                    return;

                var popupMenu = solution.GetComponent<OccurrencePopupMenu>();
                var occurrences = Search();
                if (occurrences == null || occurrences.Count == 0)
                {
                    var tooltipManager = Shell.Instance.GetComponent<ITooltipManager>();
                    Lifetime.Define(Lifetime.Eternal,
                        "Tooltip",
                        def => tooltipManager.Show(def, WindowlessControlAutomation.Create(emptyTooltip), emptyTooltipSource.Create(def.Lifetime), null,
                            null));
                    return;
                }

                var popupMenuOptions =
                    popupMenuBehaviour.GetPopupMenuOptions(iconId, this, occurrences, false);
                popupMenu.ShowMenuFromTextControl(ctx, occurrences, popupMenuOptions);
            });
        }), (Action)(() => { }), true);
    }

    public void ShowOccurrences(DocumentRange range, string emptyTooltip, IconId? iconId = null)
    {
        var source = new PopupWindowContextSource(_ => new RiderEditorOffsetPopupWindowContext(range.StartOffset.Offset));
        var emptyTooltipSource = new PopupWindowContextSource(lt =>
            new RiderEditorOffsetPopupWindowContext((range.StartOffset.Offset + range.EndOffset.Offset) / 2));
        ShowOccurrences(source, emptyTooltipSource, emptyTooltip, iconId);
    }
}

public class InlineSearchDescriptor(SearchRequest request, ICollection<IOccurrence>? occurrences = null) : SearchDescriptor(request, occurrences)
{
    public override string GetResultsTitle(OccurrenceSection section) => Request.Title;

    protected override Func<SearchRequest, IOccurrenceBrowserDescriptor> GetDescriptorFactory()
    {
        return request =>
        {
            var occurrences = request.Search();
            return new InlineSearchDescriptor(request, occurrences);
        };
    }
}