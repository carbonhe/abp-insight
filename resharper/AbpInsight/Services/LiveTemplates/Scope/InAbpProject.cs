using System;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace AbpInsight.Services.LiveTemplates.Scope;

public class InAbpProject : InLanguageSpecificProject
{
    private static readonly Guid DefaultId = new("34DB6039-873B-406F-B0B5-E452DF7236C7");
    private static readonly Guid QuickId = new("1A8435F0-E7AF-4180-A5AE-BF568D993879");

    public InAbpProject() : base(ProjectLanguage.CSHARP)
    {
        AdditionalSuperTypes.Add(typeof(InLanguageSpecificProject));
    }

    public override Guid GetDefaultUID() => DefaultId;
    public override string PresentableShortName => QuickListTitle;

    public override PsiLanguageType RelatedLanguage => CSharpLanguage.Instance!;

    public override string QuickListTitle => "Abp project";

    public override Guid QuickListUID => QuickId;
}

public class InAbpWidgetProject : InAbpProject
{
    private static readonly Guid DefaultId = new Guid("7F818B20-832E-473F-847B-C10BCA1350B9");

    public override Guid GetDefaultUID() => DefaultId;

    public override string PresentableShortName => "Abp widget project";
}