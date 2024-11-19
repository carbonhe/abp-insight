using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.Tooltips;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Feature.Services.Tree.SectionsManagement;
using JetBrains.ReSharper.Resources.Shell;

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

    public void ShowOccurrences(IDataContext context, string emptyTooltip)
    {
        var tooltipManager = Shell.Instance.GetComponent<ITooltipManager>();

        Lifetime.Using(lifetime =>
        {
            var occurrences = Search();

            var occurrencePopupMenu = context.GetComponent<OccurrencePopupMenu>();

            if (occurrences is not { Count: > 0 })
                tooltipManager.ShowIfPopupWindowContext(emptyTooltip, context);
            else
                occurrencePopupMenu.ShowMenuFromTextControl(
                    context,
                    occurrences,
                    new OccurrencePopupMenuOptions(
                        Title,
                        true,
                        new OccurrencePresentationOptions
                        {
                            TextDisplayStyle = TextDisplayStyle.ContainingType,
                            LocationStyle = GlobalLocationStyle.None
                        }, null,
                        () => CreateSearchDescriptor(occurrences)));
        });
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