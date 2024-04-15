using System;
using System.Collections.Generic;
using System.Linq;
using AbpInsight.Framework;
using AbpInsight.Resources;
using AbpInsight.Utils;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.Application.UI.Controls.GotoByName;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Resources.Icons;
using JetBrains.Rider.Backend.Platform.Icons;
using JetBrains.UI.RichText;

namespace AbpInsight.Daemon.CodeInsights;

[SolutionComponent]
public class TypeCodeInsightProvider(
    BulbMenuComponent bulbMenuComponent,
    OccurrencePopupMenu occurrencePopupMenu,
    IconHost iconHost,
    AbpInsighter insighter) : AbstractCodeInsightProvider(bulbMenuComponent, occurrencePopupMenu)
{
    public override bool AddHighlighting(ICSharpDeclaration declaration, IHighlightingConsumer consumer)
    {
        if (declaration is not IClassLikeDeclaration element)
            return false;

        var typeElement = element.DeclaredElement;
        if (typeElement != null)
        {
            if (AbpInsighter.IsAbpModuleType(typeElement))
            {
                consumer.AddImplicitUsedHighlighting(element);

                AddHighlighting(
                    consumer,
                    element,
                    typeElement,
                    "AbpModule",
                    "Abp module",
                    "Abp module",
                    iconHost.Transform(AbpIcons.AbpModule),
                    CreateAbpModuleActionFactory((IClass)typeElement, declaration.GetPsiModule()),
                    null);
            }

            if (insighter.TryGetInjectableService(typeElement,out var injectable))
            {
                consumer.AddImplicitUsedHighlighting(element);
            }
        }

        return true;
    }

    private Func<IDataContext, IEnumerable<BulbMenuItem>> CreateAbpModuleActionFactory(IClass clazz, IPsiModule psiModule)
    {
        return context =>
        {
            return
            [
                new BulbMenuItem(
                    new ExecutableItem(() => ShowDependantModules(context, clazz)),
                    new RichText("Show dependant modules"),
                    PsiFeaturesUnsortedThemedIcons.FindDependentCode.Id,
                    BulbMenuAnchors.PermanentItem),
                new BulbMenuItem(
                    new ExecutableItem(() => ShowInjectableServices(context, clazz, psiModule)),
                    new RichText($"Show injectable services in '{clazz.ShortName}'"),
                    PsiFeaturesUnsortedThemedIcons.FindDependentCode.Id,
                    BulbMenuAnchors.PermanentItem)
            ];
        };
    }

    private void ShowInjectableServices(IDataContext context, IClass clazz, IPsiModule psiModule)
    {
        ShowOccurrences(context,
            $"There is no injectable services in '{clazz.ShortName}'",
            new InlineSearchRequest($"Injectable services in '{clazz.ShortName}'", clazz.GetSolution(), new[] { clazz },
                pi =>
                {
                    var injectableServices = insighter.ScanInjectableServices(psiModule, pi);
                    return injectableServices.Select(it => new DeclaredElementOccurrence(it.Implementation)).ToArray();
                }));
    }


    private void ShowDependantModules(IDataContext context, IClass clazz)
    {
        ShowOccurrences(context,
            $"'{clazz.ShortName}' has no module dependencies",
            new InlineSearchRequest($"Dependant modules by '{clazz.ShortName}'", clazz.GetSolution(), new[] { clazz },
                pi =>
                {
                    var dependencies = new JetHashSet<IClass>(DeclaredElementEqualityComparer.TypeElementComparer) { clazz };

                    using (CompilationContextCookie.GetExplicitUniversalContextIfNotSet())
                    {
                        SearchDependantModules(clazz, dependencies);
                    }

                    dependencies.Remove(clazz);

                    return dependencies.Select(it => new DeclaredElementOccurrence(it)).ToArray();
                }));
    }

    private static void SearchDependantModules(IClass clazz, ISet<IClass> dependencies)
    {
        var attributeInstances = clazz.GetAttributeInstances(KnownTypes.DependsOnAttribute, AttributesSource.Self);
        foreach (var attributeInstance in attributeInstances)
        {
            foreach (var attributeValue in attributeInstance.PositionParameters().SelectMany(it => it.ArrayValue))
            {
                var element = attributeValue.TypeValue.GetClassType()!;
                if (AbpInsighter.IsAbpModuleType(element) && !dependencies.Contains(element))
                {
                    dependencies.Add(element);
                    SearchDependantModules(element, dependencies);
                }
            }
        }
    }
}