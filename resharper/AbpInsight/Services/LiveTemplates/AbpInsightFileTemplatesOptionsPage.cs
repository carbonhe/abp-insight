using AbpInsight.Resources;
using AbpInsight.Services.LiveTemplates.Scope;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.IDE.UI;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Settings;
using JetBrains.ReSharper.LiveTemplates.UI;
using JetBrains.Rider.Model;

namespace AbpInsight.Services.LiveTemplates;

[ZoneMarker(typeof(IRiderModelZone))]
[OptionsPage("RiderAbpInsightFileTemplatesSettings", "Abp Insight", typeof(AbpInsightIcons.Logo))]
public class AbpInsightFileTemplatesOptionsPage(
    Lifetime lifetime,
    AbpInsightProjectScopeCategoryUiProvider uiProvider,
    OptionsPageContext optionsPageContext,
    OptionsSettingsSmartContext optionsSettingsSmartContext,
    StoredTemplatesProvider storedTemplatesProvider,
    ScopeCategoryManager scopeCategoryManager,
    IDialogHost dialogHost,
    TemplatesUIFactory uiFactory,
    IconHostBase iconHostBase)
    : RiderFileTemplatesOptionPageBase(lifetime, uiProvider, optionsPageContext, optionsSettingsSmartContext,
        storedTemplatesProvider, scopeCategoryManager, uiFactory, iconHostBase, dialogHost, "CSHARP");