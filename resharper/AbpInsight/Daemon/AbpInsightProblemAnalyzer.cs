using AbpInsight.Framework;
using AbpInsight.ProjectModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace AbpInsight.Daemon;

public abstract class AbpInsightProblemAnalyzer<T>(AbpInsighter insighter) : ElementProblemAnalyzer<T>, IConditionalElementProblemAnalyzer
    where T : ITreeNode
{
    protected AbpInsighter Insighter { get; } = insighter;

    public bool ShouldRun(IFile file, ElementProblemAnalyzerData data)
    {
        if (data.GetDaemonProcessKind() == DaemonProcessKind.GLOBAL_WARNINGS)
            return false;

        if (!file.GetSolution().HasAbpReference())
            return false;

        if (data.SourceFile == null || !file.Language.Is<CSharpLanguage>())
            return false;

        return data.SourceFile.ToProjectFile()?.GetProject()?.IsProjectFromUserView() == true;
    }


    protected sealed override void Run(T element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        Analyze(element, data, consumer);
    }

    protected abstract void Analyze(T element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer);
}