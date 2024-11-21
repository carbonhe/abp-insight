using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.Controls;
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
using JetBrains.ReSharper.Resources.Shell;
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
        if (Search()?.Count > 0)
            TypeMarkOnGutterBase.ShowMenu(iconId, range, _ => this);
        else
        {
            var tooltipManager = Shell.Instance.GetComponent<ITooltipManager>();
            var source = new PopupWindowContextSource(lt =>
                new RiderEditorOffsetPopupWindowContext(range.StartOffset.Offset));
            Lifetime.Define(Lifetime.Eternal,
                "Tooltip",
                def => tooltipManager.Show(def, WindowlessControlAutomation.Create(emptyTooltip), source.Create(def.Lifetime), null, null));
        }
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