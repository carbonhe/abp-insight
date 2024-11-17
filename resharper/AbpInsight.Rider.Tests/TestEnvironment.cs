using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Rider.Backend.Env;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace AbpInsight.Rider.Tests
{
    [ZoneDefinition]
    public class AbpInsightTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IAbpInsightZone>,IRequire<IRiderPlatformZone>
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