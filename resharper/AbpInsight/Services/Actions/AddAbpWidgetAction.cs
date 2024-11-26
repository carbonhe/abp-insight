using System;
using System.Linq;
using AbpInsight.Resources;
using AbpInsight.Services.LiveTemplates.Scope;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.IDE.UI;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Extensions.Properties;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Context;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.FileTemplates;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Settings;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Templates;
using JetBrains.Rider.Model.UIAutomation;
using JetBrains.Threading;

namespace AbpInsight.Services.Actions;

[Action(typeof(Strings), nameof(Strings.AddAbpWidget_Description), Icon = typeof(AbpInsightIcons.Logo))]
public class AddAbpWidgetAction : IExecutableAction
{
    private static readonly Guid AbpWidgetTemplateId = new("E342F586-1A6C-4E66-938B-5345236042F1");

    // The action shouldn't be available in all scenarios
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
        var locations = FileTemplateUtil.GetProjectFoldersFromContext(context);
        if (locations.Length == 0)
        {
            presentation.Visible = false;
            return false;
        }

        var scopeIds = locations
            .SelectMany(it => TemplateScopeManager.Instance.EnumerateRealScopePoints(new TemplateAcceptanceContext(it)))
            .Select(it => it.GetDefaultUID())
            .ToArray();

        if (new[] { InAbpWidgetProject.Id }.AsParallel().All(it => scopeIds.Contains(it)))
        {
            presentation.Visible = true;
            return true;
        }

        presentation.Visible = false;
        return false;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
        var template = GetTemplate(context, AbpWidgetTemplateId);
        var locations = FileTemplateUtil.GetProjectFoldersFromContext(context);

        // TODO: Collect the options from user inputs
        var spanGrid = BeControls.GetSpanGrid("auto,*");
        context.GetComponent<IDialogHost>().Show(lifetime =>
        {
            context.Prolongate(lifetime);
            var widgetNameControl = BeControls.GetTextBox(lifetime);
            spanGrid.AddColumnElementToNewRow(widgetNameControl.WithDescription("Abp widget name", lifetime));
            var ellipsis = "And any other options...".GetBeLabel();
            spanGrid.AddColumnElementToNewRow(ellipsis);
            var dialog = spanGrid.InDialog("Generate Abp widget", "AbpWidget", DialogModality.MODAL, BeControlSizes.GetSize(BeControlSizeType.LARGE));
            return dialog.WithOkButton(lifetime, () => { GenerateFiles(locations, template, widgetNameControl.GetText()); });
        });
    }


    // TODO: We are able to generate multiple files if needed.
    private static void GenerateFiles(ProjectFolderWithLocation[] locations, Template template, string widgetName)
    {
        FileTemplatesManager.Instance.CreateFileFromTemplateAsync($"{widgetName}.cs", locations, template).NoAwait();
    }

    private static Template GetTemplate(IDataContext context, Guid templateId)
    {
        var store = context.GetComponent<ISettingsStore>().BindToContextTransient(ContextRange.ApplicationWide);
        return StoredTemplatesProvider.Instance.GetTemplate(store, templateId)!;
    }
}