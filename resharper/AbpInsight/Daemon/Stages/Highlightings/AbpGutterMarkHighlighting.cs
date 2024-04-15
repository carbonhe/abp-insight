using System.Collections.Generic;
using AbpInsight.Daemon.Errors;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.Application.UI.Icons.CommonThemedIcons;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Resources.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace AbpInsight.Daemon.Stages.Highlightings;

public interface IAbpGutterMarkHighlighting : ICustomAttributeIdHighlighting, IAbpIndicatorHighlighting
{
    IEnumerable<BulbMenuItem> Actions { get; }
}

[StaticSeverityHighlighting(Severity.INFO, typeof(AbpGutterMarks), Languages = CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE)]
public class AbpModuleGutterMarkHighlighting(IClassDeclaration classDeclaration, IEnumerable<BulbMenuItem> actions) : IAbpGutterMarkHighlighting
{
    public string ToolTip => "Abp Module";
    public string ErrorStripeToolTip => ToolTip;
    public string AttributeId => AbpHighlightingAttributeIds.ModuleGutterIcon;

    public IEnumerable<BulbMenuItem> Actions { get; } = actions;

    public bool IsValid() => classDeclaration.IsValid();

    public DocumentRange CalculateRange() => classDeclaration.GetNameDocumentRange();
}