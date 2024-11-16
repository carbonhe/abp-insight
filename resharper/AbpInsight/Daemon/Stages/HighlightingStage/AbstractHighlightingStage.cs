using System;
using System.Collections.Generic;
using AbpInsight.Daemon.Stages.Highlightings;
using AbpInsight.Framework;
using AbpInsight.ProjectModel;
using JetBrains.Application.Settings;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AbpInsight.Daemon.Stages.HighlightingStage;

public abstract class AbstractHighlightingStage(
    AbpInsighter abpInsighter,
    IEnumerable<IAbpHighlightingProvider> highlightingProviders) : CSharpDaemonStageBase
{
    protected override IDaemonStageProcess? CreateProcess(
        IDaemonProcess process,
        IContextBoundSettingsStore settings,
        DaemonProcessKind processKind,
        ICSharpFile file)
    {
        return new HighlightingProcess(process, file, abpInsighter, highlightingProviders, processKind);
    }

    protected override bool IsSupported(IPsiSourceFile? sourceFile)
    {
        if (sourceFile == null || !sourceFile.IsValid())
            return false;
        return sourceFile.GetSolution().HasAbpReference() && sourceFile.IsLanguageSupported<CSharpLanguage>();
    }
}

public class HighlightingProcess(
    IDaemonProcess process,
    ICSharpFile file,
    AbpInsighter insighter,
    IEnumerable<IAbpHighlightingProvider> highlightingProviders,
    DaemonProcessKind processKind) : CSharpDaemonStageProcessBase(process, file)
{
    private readonly ISet<IDeclaredElement> _markedDeclarations = new HashSet<IDeclaredElement>();

    public override void Execute(Action<DaemonStageResult> committer)
    {
        var consumer = new FilteringHighlightingConsumer(DaemonProcess.SourceFile, File, DaemonProcess.ContextBoundSettingsStore);

        File.ProcessThisAndDescendants(this, consumer);

        committer(new DaemonStageResult(consumer.CollectHighlightings()));
    }


    public override void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer consumer)
    {
        if (element is not ICSharpDeclaration declaration)
            return;

        foreach (var provider in highlightingProviders)
        {
            if (provider.AddHighlighting(declaration, consumer))
            {
                _markedDeclarations.Add(declaration.DeclaredElement.NotNull());
            }
        }
    }
}