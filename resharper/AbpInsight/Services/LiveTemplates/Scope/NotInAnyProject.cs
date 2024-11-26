using System;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;

namespace AbpInsight.Services.LiveTemplates.Scope;

/// <summary>
/// This scope used to hide templates in "Add" menu.
/// </summary>
public class NotInAnyProject : TemplateScopePoint
{
    public override Guid GetDefaultUID() => new("DFC3ADF3-082F-495F-94F9-D7A54E074BC9");
}