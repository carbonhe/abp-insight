using System.Collections.Generic;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DocumentMarkup;
using JetBrains.UI.Icons;
using JetBrains.Util;

namespace AbpInsight.Daemon.Stages.Highlightings;

public abstract class AbstractAbpInsightGutterMark(IconId iconId) : IconGutterMarkType(iconId)
{
    public override IAnchor Priority => BulbMenuAnchors.PermanentItem;


    public override IEnumerable<BulbMenuItem> GetBulbMenuItems(IHighlighter highlighter)
    {
        var solution = Shell.Instance.GetComponent<SolutionsManager>().Solution;
        if (solution == null)
            return EmptyList<BulbMenuItem>.InstanceList;

        var highlighting = highlighter.GetHighlighting();

        var items = (highlighting as IAbpInsightGutterMarkHighlighting)?.MenuItems;
        return items ?? EmptyList<BulbMenuItem>.InstanceList;
    }
}