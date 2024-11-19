using AbpInsight.Daemon.Stages.Highlightings;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.Analysis;

[TestAbpInsight]
public class ModuleTypeProblemAnalyzerTests : CSharpHighlightingTestBase<CSharpAbpHighlightingBase>
{
    protected override string RelativeTestDataPath => @"Analysis";


    [Test]
    public void ModuleTypeProblemAnalyzer() => DoNamedTest();
}