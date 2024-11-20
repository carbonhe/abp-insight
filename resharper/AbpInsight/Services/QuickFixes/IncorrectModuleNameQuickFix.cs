using System;
using AbpInsight.Daemon.Errors;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Intentions.ContextActions.Scoped;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AbpInsight.Services.QuickFixes;

[QuickFix]
public class IncorrectModuleNameQuickFix(IncorrectModuleNamingWarning warning) : AbpInsightQuickFixBase
{
    private string SuggestedName => $"{warning.ClassDeclaration.NameIdentifier.Name}Module";
    private IDeclaredElement? DeclaredElement => warning.ClassDeclaration.DeclaredElement;

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        return RenameIntentionUtils.Rename(solution, warning.ClassDeclaration.DeclaredElement.NotNull(), SuggestedName);
    }


    public override bool IsAvailable(IUserDataHolder cache)
    {
        return RenameIntentionUtils.IsNewNameValid(DeclaredElement, SuggestedName);
    }


    public override string Text => $"Rename to '{SuggestedName}'";
}