using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace AbpInsight.Tests
{
    [ZoneDefinition]
    public class AbpInsightTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IAbpInsightZone>
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>,
        IRequire<AbpInsightTestEnvironmentZone>
    {
    }

    [SetUpFixture]
    public class AbpInsightTestsAssembly : ExtensionTestEnvironmentAssembly<AbpInsightTestEnvironmentZone>
    {
    }
}