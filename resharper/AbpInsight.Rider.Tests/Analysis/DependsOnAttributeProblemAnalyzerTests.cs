using AbpInsight.Daemon.Stages.Highlightings;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.Analysis;

[TestAbpInsight]
public class DependsOnAttributeProblemAnalyzerTests : CSharpHighlightingTestBase<CSharpAbpInsightHighlightingBase>
{
    [Test]
    public void DependsOnAttributeProblemAnalyzer() => DoNamedTest();
}