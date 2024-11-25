using System.Collections.Generic;
using System.Linq;
using AbpInsight.ProjectModel;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Context;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;

namespace AbpInsight.Services.LiveTemplates.Scope;

[ShellComponent(Instantiation.DemandAnyThreadSafe)]
public class AbpProjectScopeProvider : ScopeProvider
{
    public AbpProjectScopeProvider()
    {
        Creators.Add(TryToCreate<InAbpProject>);
        Creators.Add(TryToCreate<InAbpWidgetProject>);
    }

    public override IEnumerable<ITemplateScopePoint> ProvideScopePoints(TemplateAcceptanceContext context)
    {
        if (!context.Solution.HasAbpReference())
            yield break;

        var project = context.GetProject();
        if (project == null || !project.IsAbpProject())
            yield break;

        yield return new InAbpProject();

        var frameworkId = project.GetCurrentTargetFrameworkId();
        if (project.GetModuleReferences(frameworkId).Any(reference => reference.Name == "Volo.Abp.AspNetCore.Mvc.UI.Widgets"))
        {
            yield return new InAbpWidgetProject();
        }
    }
}