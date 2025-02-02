﻿using AbpInsight.Daemon.Errors;
using AbpInsight.VoloAbp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AbpInsight.Daemon.Stages.Analysis;

[ElementProblemAnalyzer(
    typeof(IClassDeclaration),
    HighlightingTypes =
    [
        typeof(IncorrectModuleNamingWarning),
        typeof(ModuleTypeMustBePublicError),
        typeof(ModuleContainsParameterizedConstructorError)
    ])]
public class ModuleTypeProblemAnalyzer(AbpInsighter insighter) : AbpInsightProblemAnalyzer<IClassDeclaration>()
{
    protected override void Analyze(IClassDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (!AbpInsighter.IsAbpModuleType(element.DeclaredElement))
            return;


        if (!element.NameIdentifier.Name.EndsWith("Module"))
            consumer.AddHighlighting(new IncorrectModuleNamingWarning(element));


        if (element.GetAccessRights() != AccessRights.PUBLIC)
            consumer.AddHighlighting(new ModuleTypeMustBePublicError(element));


        foreach (var constructorDeclaration in element.ConstructorDeclarations.Where(it => it.ParameterDeclarations.Any()))
        {
            consumer.AddHighlighting(new ModuleContainsParameterizedConstructorError(constructorDeclaration));
        }
    }
}