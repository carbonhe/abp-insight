using System.Collections.Generic;
using AbpInsight.Resources;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Icons;
using JetBrains.Util;

namespace AbpInsight.Daemon.Stages.Highlightings;

public abstract class AbpGutterMarkType(IconId iconId) : IconGutterMarkType(iconId)
{
    public override IAnchor Priority => BulbMenuAnchors.PermanentItem;


    public override IEnumerable<BulbMenuItem> GetBulbMenuItems(IHighlighter highlighter)
    {
        var solution = Shell.Instance.GetComponent<SolutionsManager>().Solution;
        if (solution == null)
            return EmptyList<BulbMenuItem>.InstanceList;

        var daemon = solution.GetComponent<IDaemon>();
        var highlighting = daemon.GetHighlighting(highlighter);

        var items = (highlighting as IAbpGutterMarkHighlighting)?.Actions;
        return items ?? EmptyList<BulbMenuItem>.InstanceList;
    }

    public override IGutterMarkHoverHandler? GetHoverHandler(IHighlighter highlighter)
    {
        var solution = Shell.Instance.GetComponent<SolutionsManager>().Solution;
        if (solution == null)
            return null;

        var daemon = solution.GetComponent<IDaemon>();
        var highlighting = daemon.GetHighlighting(highlighter);

        return highlighting as IGutterMarkHoverHandler;
    }
}

public class AbpModuleGutterMarkType() : AbpGutterMarkType(AbpInsightIcons.AbpModule);