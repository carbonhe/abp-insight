using System.Drawing;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.TextControl.DocumentMarkup;

namespace AbpInsight.Daemon.Stages.Highlightings;

[RegisterHighlighterGroup(GroupId, "AbpInsight", HighlighterGroupPriority.LANGUAGE_SETTINGS, Language = typeof(CSharpLanguage))]
[RegisterHighlighter(AbpInsightImplicitlyUsedIdentifier, EffectType = EffectType.TEXT, FontStyle = FontStyle.Bold,
    Layer = HighlighterLayer.SYNTAX + 1)]
[RegisterHighlighter(
    AbpInsightGutterIcon,
    EffectType = EffectType.GUTTER_MARK,
    GutterMarkType = typeof(AbpInsightGutterMark),
    Layer = HighlighterLayer.SYNTAX + 1)]
public static class AbpInsightHighlightingAttributeIds
{
    public const string GroupId = "AbpInsight";

    // All attributes should begin with "ReSharper Cg ". See CgHighlighterNamesProvider below
    public const string AbpInsightImplicitlyUsedIdentifier = "ReSharper AbpInsight Implicitly Used Identifier";

    // Icons
    public const string AbpInsightGutterIcon = "AbpInsight Gutter Icon";
}