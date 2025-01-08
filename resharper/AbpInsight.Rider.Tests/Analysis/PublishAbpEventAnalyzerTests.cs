using AbpInsight.Daemon.Stages.Highlightings;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.Analysis;

[TestAbpInsight]
public class PublishAbpEventAnalyzerTests : CSharpHighlightingTestBase<CSharpAbpInsightHighlightingBase>
{
    [Test]
    public void PublishAbpEventAnalyzer() => DoNamedTest();
}