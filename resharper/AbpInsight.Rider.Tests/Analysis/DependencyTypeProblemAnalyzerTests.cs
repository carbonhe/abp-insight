using AbpInsight.Daemon.Stages.Highlightings;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.Analysis;

[TestAbpInsight]
public class DependencyTypeProblemAnalyzerTests : CSharpHighlightingTestBase<CSharpAbpInsightHighlightingBase>
{
    [Test]
    public void DependencyTypeProblemAnalyzer() => DoNamedTest();
}