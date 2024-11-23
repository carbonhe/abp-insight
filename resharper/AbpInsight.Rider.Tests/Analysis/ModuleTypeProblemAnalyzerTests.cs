using AbpInsight.Daemon.Stages.Highlightings;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.Analysis;

[TestAbpInsight]
public class ModuleTypeProblemAnalyzerTests : CSharpHighlightingTestBase<CSharpAbpInsightHighlightingBase>
{
    [Test]
    public void ModuleTypeProblemAnalyzer() => DoNamedTest();
}