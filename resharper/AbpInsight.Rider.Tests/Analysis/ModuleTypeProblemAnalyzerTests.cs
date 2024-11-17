using AbpInsight.Daemon.Errors;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Projects;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.Analysis;

[TestPackages("Volo.Abp.Core/8.0.0")]
[TestNetCore20]
public class ModuleTypeProblemAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analysis";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
    {
        return highlighting is IncorrectModuleNamingWarning;
    }
    
    

    [Test]
    public void TestModuleTypeProblemAnalyzer()
    {
        DoNamedTest2();
    }
}