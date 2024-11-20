using System.Collections.Generic;
using System.Linq;
using AbpInsight.Framework;
using AbpInsight.Resources;
using AbpInsight.Utils;
using JetBrains.Application.Parts;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.Specific.InheritedGutterMark;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Resources.Icons;

namespace AbpInsight.Daemon.Stages.Highlightings.Providers;

[SolutionComponent(Instantiation.DemandAnyThreadUnsafe)]
public class TypeDetector(AbpInsighter insighter) : IAbpDeclarationHighlightingProvider
{
    public bool AddDeclarationHighlighting(IDeclaration treeNode, IHighlightingConsumer consumer)
    {
        if (treeNode is not IClassDeclaration declaration)
            return false;
        var typeElement = declaration.DeclaredElement;

        if (typeElement != null)
        {
            if (AbpInsighter.IsAbpModuleType(typeElement))
            {
                AddModuleTypeHighlighting(consumer, declaration, (IClass)typeElement);
            }
        }

        return false;
    }


    private void AddModuleTypeHighlighting(IHighlightingConsumer consumer, IClassDeclaration declaration, IClass clazz)
    {
        consumer.AddImplicitConfigurableHighlighting(declaration);


        consumer.AddHighlighting(
            new AbpInsightGutterMarkHighlighting(
                declaration,
                "",
                CreateModuleTypeBulbItems(declaration.GetNameDocumentRange(), clazz)));
    }

    private static IEnumerable<BulbMenuItem> CreateModuleTypeBulbItems(DocumentRange documentRange, IClass clazz)
    {
        yield return new BulbMenuItem(
            new ExecutableItem(() =>
            {
                var searchRequest = new InlineSearchRequest(
                    $"Dependant modules by '{clazz.ShortName}'",
                    clazz.GetSolution(),
                    new[] { clazz },
                    pi =>
                    {
                        var dependencies = new JetHashSet<IClass>(DeclaredElementEqualityComparer.TypeElementComparer) { clazz };

                        using (CompilationContextCookie.GetExplicitUniversalContextIfNotSet())
                        {
                            SearchDependantModules(clazz, dependencies);
                        }

                        dependencies.Remove(clazz);

                        return dependencies.Select(it => new DeclaredElementOccurrence(it)).ToArray();
                    }
                );
                TypeMarkOnGutterBase.ShowMenu(AbpInsightIcons.Logo.Id, documentRange, _ => searchRequest);
            }),
            "Show dependant modules",
            PsiFeaturesUnsortedThemedIcons.FindDependentCode.Id,
            AbpInsightAnchors.BulbGroup);
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