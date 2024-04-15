using System.Collections.Generic;
using System.Linq;
using AbpInsight.Framework.Injectable;
using AbpInsight.Utils;
using JetBrains.Application.Parts;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace AbpInsight.Framework;

[SolutionComponent(Instantiation.DemandAnyThread)]
public class AbpInsighter(IEnumerable<InjectableServiceProviderBase> injectableProviders)
{
    public static bool IsAbpModuleType(ITypeElement? typeElement)
    {
        return typeElement is IClass { IsAbstract: false } clazz && clazz.DerivesFrom(KnownTypes.IAbpModule);
    }

    public IEnumerable<InjectableService> ScanInjectableServices(IPsiModule psiModule, IProgressIndicator progressIndicator)
    {
        return injectableProviders.SelectMany(provider => provider.Scan(psiModule, progressIndicator));
    }

    public bool TryGetInjectableService(ITypeElement type, out InjectableService? injectable)
    {
        foreach (var provider in injectableProviders)
        {
            if (provider.TryGet(type, out injectable))
            {
                return true;
            }
        }

        injectable = null;
        return false;
    }
}