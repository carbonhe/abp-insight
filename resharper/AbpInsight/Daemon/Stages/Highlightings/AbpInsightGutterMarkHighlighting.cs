using System.Collections.Generic;
using AbpInsight.Daemon.Errors;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AbpInsight.Daemon.Stages.Highlightings;

[StaticSeverityHighlighting(Severity.INFO, typeof(AbpInsightGutterMarks), Languages = CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.NONE)]
public class AbpInsightGutterMarkHighlighting(IClassDeclaration classDeclaration, string tooltip, IEnumerable<BulbMenuItem>? menuItems = null)
    : IAbpInsightGutterMarkHighlighting
{
    public string ToolTip => tooltip;
    public string ErrorStripeToolTip => ToolTip;
    public string AttributeId => AbpInsightHighlightingAttributeIds.ABPINSIGHT_GUTTER_ICON;

    public IEnumerable<BulbMenuItem>? MenuItems => menuItems;

    public bool IsValid() => classDeclaration.IsValid();

    public DocumentRange CalculateRange() => classDeclaration.GetNameDocumentRange();
}