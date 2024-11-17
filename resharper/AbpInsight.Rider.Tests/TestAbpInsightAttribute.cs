using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Projects;
using JetBrains.Util.Dotnet.TargetFrameworkIds;
using NuGet.Frameworks;
using NuGet.Packaging.Core;

namespace AbpInsight.Rider.Tests;

public class TestAbpInsightAttribute(Version version)
    : TestAspectAttribute, ITestTargetFrameworkIdProvider, ITestPackagesProvider, IReuseSolutionScopeAttribute
{
    private static readonly Version DefaultVersion = new(8, 0);

    public TestAbpInsightAttribute() : this(DefaultVersion)
    {
    }

    public TargetFrameworkId GetTargetFrameworkId() => TargetFrameworkId.Create(FrameworkConstants.CommonFrameworks.NetCoreApp20);

    public IEnumerable<PackageDependency> GetPackages(TargetFrameworkId targetFrameworkId)
    {
        return GetPackageNames().Select(it => TestPackagesAttribute.ParsePackageDependency($"{it}/{version.Major}.{version.Minor}.0"));
    }

    public string ScopeName => version.ToString();


    private static IEnumerable<string> GetPackageNames()
    {
        yield return "Volo.Abp.Core";
    }

    public bool Inherits => false;
}