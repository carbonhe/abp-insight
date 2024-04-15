using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Daemon.SolutionAnalysis;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;

namespace AbpInsight;

[ZoneDefinition(ZoneFlags.AutoEnable)]
public interface IAbpInsightZone : IZone,
    IRequire<DaemonZone>,
    IRequire<ILanguageCSharpZone>,
    IRequire<PsiFeaturesImplZone>,
    IRequire<SweaZone>;

[ZoneMarker]
public class ZoneMarker : IRequire<IAbpInsightZone>;