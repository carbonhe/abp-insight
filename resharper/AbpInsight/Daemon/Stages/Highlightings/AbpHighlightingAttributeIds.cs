using System.Drawing;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.TextControl.DocumentMarkup;

namespace AbpInsight.Daemon.Stages.Highlightings;

[RegisterHighlighterGroup(GroupId, "Abp", HighlighterGroupPriority.LANGUAGE_SETTINGS, Language = typeof(CSharpLanguage))]
[RegisterHighlighter(ImplicitlyUsedIdentifier, EffectType = EffectType.TEXT, FontStyle = FontStyle.Bold, Layer = HighlighterLayer.SYNTAX + 1)]
[RegisterHighlighter(
    ModuleGutterIcon,
    EffectType = EffectType.GUTTER_MARK,
    GutterMarkType = typeof(AbpModuleGutterMarkType),
    Layer = HighlighterLayer.SYNTAX + 1)]
public static class AbpHighlightingAttributeIds
{
    public const string GroupId = "Abp";

    public const string ImplicitlyUsedIdentifier = "Abp Implicitly Used Identifier";

    // Icons
    public const string ModuleGutterIcon = "Abp Module Gutter Icon";
}