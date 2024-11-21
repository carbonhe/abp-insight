using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
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
using JetBrains.ReSharper.Feature.Services.Navigation.Options;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Feature.Services.Tree.SectionsManagement;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.CodeWithMe;
using JetBrains.TextControl.Coords;
using JetBrains.TextControl.DataContext;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.TextControl.TextControlsManagement;
using JetBrains.UI.Icons;

namespace AbpInsight.Utils;

public class InlineSearchRequest(
    string title,
    ISolution solution,
    ICollection searchTargets,
    Func<IProgressIndicator, ICollection<IOccurrence>> search) : SearchRequest
{
    public InlineSearchRequest(string title,
        ISolution solution,
        ICollection searchTargets,
        ICollection<IOccurrence> occurrences) : this(title, solution, searchTargets, _ => occurrences)
    {
    }

    public override string Title => title;

    public override ISolution Solution => solution;

    public override ICollection SearchTargets => searchTargets;

    public override ICollection<IOccurrence> Search(IProgressIndicator progressIndicator) => search(progressIndicator);

    public SearchDescriptor CreateSearchDescriptor(ICollection<IOccurrence>? occurrences = null) => new InlineSearchDescriptor(this, occurrences);

    public void ShowOccurrences(DocumentRange range, string emptyTooltip, IconId? iconId = null)
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
            using (CompilationContextCookie.GetOrCreate(range.Document.GetContext(solution)))
                Lifetime.Using(lifetime =>
                {
                    var dataRules = DataRules.AddRule<PopupWindowContextSource>("GutterMarkWindowSource",
                        UIDataConstants.PopupWindowContextSource, GuessPopupWindowContext(range));
                    var ctx = dataContexts.CreateOnActiveControl(lifetime, dataRules);
                    if (textControlPos == null ||
                        !textControlPos.Equals(ctx.GetData<ITextControlPos>(TextControlDataConstants.TextControlPosition)))
                        return;
                    var skipMenuIfSingleEnabled = solution.GetComponent<ISettingsStore>()
                        .GetValue<SearchAndNavigationSettings, bool>(ctx,
                            SearchAndNavigationOptions.GotoSingleHierarchyItemImmediatelyExpression);
                    var component3 = solution.GetComponent<OccurrencePopupMenu>();
                    var occurrences = Search();
                    if (occurrences == null || occurrences.Count == 0)
                    {
                        var tooltipManager = Shell.Instance.GetComponent<ITooltipManager>();
                        var source = new PopupWindowContextSource(lt =>
                            new RiderEditorOffsetPopupWindowContext(range.StartOffset.Offset));
                        Lifetime.Define(Lifetime.Eternal,
                            "Tooltip",
                            def => tooltipManager.Show(def, WindowlessControlAutomation.Create(emptyTooltip), source.Create(def.Lifetime), null,
                                null));
                        return;
                    }

                    var popupMenuOptions =
                        popupMenuBehaviour.GetPopupMenuOptions(iconId, this, occurrences, skipMenuIfSingleEnabled);
                    component3.ShowMenuFromTextControl(ctx, occurrences, popupMenuOptions);
                });
        }), (Action)(() => { }), true);
    }


    private static PopupWindowContextSource GuessPopupWindowContext(DocumentRange range)
    {
        PopupWindowContextSource? windowContextSource = null;
        var textControl = Shell.Instance.GetComponent<TextControlManager>().CurrentFrameTextControlPerClient.ForCurrentClient();
        if (textControl != null)
            windowContextSource = Shell.Instance.GetComponent<GutterMarkMenuLayouter>().GetPopupWindowContextForLine(textControl, range);
        if (windowContextSource == null)
            windowContextSource = Shell.Instance.GetComponent<IMainWindowPopupWindowContext>().Source;
        return windowContextSource;
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