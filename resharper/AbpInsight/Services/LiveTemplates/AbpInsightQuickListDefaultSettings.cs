using System;
using System.Collections.Generic;
using AbpInsight.Services.LiveTemplates.Scope;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Implementation;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Scope;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.Settings;
using JetBrains.Util;

namespace AbpInsight.Services.LiveTemplates;

[ShellComponent(Instantiation.DemandAnyThreadSafe)]
public class AbpInsightQuickListDefaultSettings : HaveDefaultSettings<QuickListSettings>
{
    private readonly AbpInsightProjectScopeCategoryUiProvider _projectScopeProvider;
    private readonly ISettingsSchema _settingsSchema;
    private readonly ILogger _logger;

    public AbpInsightQuickListDefaultSettings(
        AbpInsightProjectScopeCategoryUiProvider projectScopeProvider,
        ISettingsSchema settingsSchema,
        ILogger logger) : base(settingsSchema, logger)
    {
        _projectScopeProvider = projectScopeProvider;
        _settingsSchema = settingsSchema;
        _logger = logger;
    }

    public override void InitDefaultSettings(ISettingsStorageMountPoint mountPoint)
    {
        var projectMountPoint = _projectScopeProvider.MainPoint.NotNull();
        InitialiseQuickList(mountPoint, projectMountPoint);

        var pos = 0;
        AddToQuickList(mountPoint, projectMountPoint, "Abp Widget", ++pos, "E342F586-1A6C-4E66-938B-5345236042F1");
    }

    private void InitialiseQuickList(ISettingsStorageMountPoint mountPoint, IMainScopePoint quickList)
    {
        var settings = new QuickListSettings { Name = quickList.QuickListTitle };
        SetIndexedKey(mountPoint, settings, new GuidIndex(quickList.QuickListUID));
    }

    private void AddToQuickList(ISettingsStorageMountPoint mountPoint, IMainScopePoint quickList, string name, int position, string guid)
    {
        var quickListKey = _settingsSchema.GetIndexedKey<QuickListSettings>();
        var entryKey = _settingsSchema.GetIndexedKey<EntrySettings>();
        var dictionary = new Dictionary<SettingsKey, object>
        {
            { quickListKey, new GuidIndex(quickList.QuickListUID) },
            { entryKey, new GuidIndex(new Guid(guid)) }
        };

        if (!ScalarSettingsStoreAccess.IsIndexedKeyDefined(mountPoint, entryKey, dictionary, null, _logger))
            ScalarSettingsStoreAccess.CreateIndexedKey(mountPoint, entryKey, dictionary, null, _logger);
        SetValue(mountPoint, (EntrySettings e) => e.EntryName, name, dictionary);
        SetValue(mountPoint, (EntrySettings e) => e.Position, position, dictionary);
    }

    public override string Name => "Abp Insight QuickList settings";
}