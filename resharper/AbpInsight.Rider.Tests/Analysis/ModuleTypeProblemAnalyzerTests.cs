using AbpInsight.Daemon.Errors;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.Analysis;

[TestAbpInsight]
public class ModuleTypeProblemAnalyzerTests : CSharpHighlightingTestBase<IncorrectModuleNamingWarning>
{
    protected override string RelativeTestDataPath => @"Analysis";


    [Test]
    public void ModuleTypeProblemAnalyzer() => DoNamedTest();
}