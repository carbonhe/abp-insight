using System.IO;
using System.Reflection;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Settings;

namespace AbpInsight.Services.LiveTemplates;

[ShellComponent(Instantiation.DemandAnyThreadSafe)]
public class AbpInsightTemplatesDefaultSettings : IHaveDefaultSettingsStream, IDefaultSettingsRootKey<LiveTemplatesSettings>
{
    public Stream GetDefaultSettingsStream(Lifetime lifetime)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AbpInsight.Templates.templates.dotSettings");
        Assertion.AssertNotNull(stream);
        lifetime.AddDispose(stream);
        return stream;
    }

    public string Name => "Abp Insight default templates";
}