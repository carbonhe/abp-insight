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
using JetBrains.Application.UI.Tooltips;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Resources.Icons;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Rider.Backend.Platform.Icons;
using JetBrains.UI.RichText;

namespace AbpInsight.Daemon.CodeInsights;

// [SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public class TypeCodeInsightProvider(
    BulbMenuComponent bulbMenuComponent,
    OccurrencePopupMenu occurrencePopupMenu,
    IconHost iconHost,
    AbpInsighter insighter)
{
    public bool AddHighlighting(IDeclaration treeNode, IHighlightingConsumer consumer)
    {
        if (treeNode is not IClassLikeDeclaration element)
            return false;

        var typeElement = element.DeclaredElement;
        if (typeElement != null)
        {
            if (AbpInsighter.IsAbpModuleType(typeElement))
            {
                consumer.AddImplicitConfigurableHighlighting(element);

                // AddHighlighting(
                //     consumer,
                //     element,
                //     typeElement,
                //     "AbpModule",
                //     "Abp module",
                //     "Abp module",
                //     iconHost.Transform(AbpInsightIcons.AbpModule),
                //     null, //CreateAbpModuleActionFactory((IClass)typeElement, treeNode.GetPsiModule()),
                //     null);
            }

            if (insighter.TryGetInjectableService(typeElement, out var injectable))
            {
                consumer.AddImplicitConfigurableHighlighting(element);
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

    protected virtual void ShowOccurrences(IDataContext context, string emptyTooltip, InlineSearchRequest inlineSearchRequest)
    {
        var tooltipManager = Shell.Instance.GetComponent<ITooltipManager>();

        Lifetime.Using(lifetime =>
        {
            var occurrences = inlineSearchRequest.Search();


            if (occurrences is not { Count: > 0 })
                tooltipManager.ShowIfPopupWindowContext(emptyTooltip, context);
            else
                occurrencePopupMenu.ShowMenuFromTextControl(
                    context,
                    occurrences,
                    new OccurrencePopupMenuOptions(
                        inlineSearchRequest.Title,
                        true,
                        new OccurrencePresentationOptions
                        {
                            TextDisplayStyle = TextDisplayStyle.ContainingType,
                            LocationStyle = GlobalLocationStyle.None
                        }, null, () => inlineSearchRequest.CreateSearchDescriptor(occurrences)));
        });
    }
}