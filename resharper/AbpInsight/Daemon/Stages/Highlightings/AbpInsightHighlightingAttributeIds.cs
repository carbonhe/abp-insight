using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.TextControl.DocumentMarkup;

namespace AbpInsight.Daemon.Stages.Highlightings;

[RegisterHighlighterGroup(GROUP_ID, "AbpInsight",
    HighlighterGroupPriority.LANGUAGE_SETTINGS, Language = typeof(CSharpLanguage), RiderNamesProviderType = typeof(AbpInsightHighlighterNamesProvider))]
[RegisterHighlighter(ABPINSIGHT_IMPLICITLY_USED_IDENTIFIER, GroupId = GROUP_ID, EffectType = EffectType.TEXT, FontStyle = FontStyle.Bold, Layer = HighlighterLayer.SYNTAX + 1)]
[RegisterHighlighter(ABPINSIGHT_GUTTER_ICON, GroupId = GROUP_ID, EffectType = EffectType.GUTTER_MARK, GutterMarkType = typeof(AbpInsightGutterMark),
    Layer = HighlighterLayer.SYNTAX + 1)]
[RegisterHighlighter(ABPINSIGHT_PUBLISH_EVENT, GroupId = GROUP_ID, EffectType = EffectType.SOLID_UNDERLINE,
    ForegroundColor = "#2AD4FF", DarkForegroundColor = "#2AD4FF", NotRecyclable = true, Layer = HighlighterLayer.ADDITIONAL_SYNTAX)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class AbpInsightHighlightingAttributeIds
{
    public const string GROUP_ID = "AbpInsight";

    public const string ABPINSIGHT_IMPLICITLY_USED_IDENTIFIER = "ReSharper AbpInsight Implicitly Used Identifier";

    public const string ABPINSIGHT_GUTTER_ICON = "ReSharper AbpInsight Gutter Icon";

    public const string ABPINSIGHT_PUBLISH_EVENT = "ReSharper AbpInsight Publish Abp Event";
}

public class AbpInsightHighlighterNamesProvider() : PrefixBasedSettingsNamesProvider("ReSharper AbpInsight", "ABPINSIGHT")
{
    public override string GetExternalName(string attributeId)
    {
        return GetHighlighterTag(attributeId);
    }
}