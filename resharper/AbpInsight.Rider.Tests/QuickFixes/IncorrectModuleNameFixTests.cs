using AbpInsight.Services.QuickFixes;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;

namespace AbpInsight.Rider.Tests.QuickFixes;

[TestAbpInsight]
public class IncorrectModuleNameFixAvailabilityTests : QuickFixAvailabilityTestBase<IncorrectModuleNameQuickFix>
{
    protected override string RelativeTestDataPath => @"QuickFixes\IncorrectModuleName\Availability";

    [Test]
    public void IncorrectModuleName() => DoNamedTest();
}

[TestAbpInsight]
public class IncorrectModuleNameFixTests : CSharpQuickFixTestBase<IncorrectModuleNameQuickFix>
{
    protected override string RelativeTestDataPath => @"QuickFixes\IncorrectModuleName";

    [Test]
    public void Test01() => DoNamedTest();

    [Test]
    public void Test02() => DoNamedTest();
}