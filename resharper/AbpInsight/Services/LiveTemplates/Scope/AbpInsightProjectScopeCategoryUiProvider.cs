using System.Collections.Generic;
using AbpInsight.Resources;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Templates;

namespace AbpInsight.Services.LiveTemplates.Scope;

[ScopeCategoryUIProvider(Priority = Priority, ScopeFilter = ScopeFilter.Project)]
public class AbpInsightProjectScopeCategoryUiProvider : ScopeCategoryUIProvider
{
    static AbpInsightProjectScopeCategoryUiProvider()
    {
        TemplateImage.Register("AbpInsight", AbpInsightIcons.Logo.Id);
    }

    private const int Priority = -100;

    public AbpInsightProjectScopeCategoryUiProvider() : base(AbpInsightIcons.Logo.Id)
    {
        MainPoint = new InAbpProject();
    }

    public override IEnumerable<ITemplateScopePoint> BuildAllPoints()
    {
        yield return new InAbpProject();
        yield return new InAbpWidgetProject();
    }

    public override string CategoryCaption => "Abp Insight";
}