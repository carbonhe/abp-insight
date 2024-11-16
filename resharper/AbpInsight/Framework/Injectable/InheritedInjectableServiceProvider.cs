using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using AbpInsight.Utils;
using JetBrains.Application.Parts;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Search;

namespace AbpInsight.Framework.Injectable;

[SolutionComponent(Instantiation.DemandAnyThreadSafe)]
public class InheritedInjectableServiceProvider : InjectableServiceProviderBase
{
    public override IEnumerable<InjectableService> Scan(IPsiModule psiModule, IProgressIndicator progressIndicator)
    {
        var finder = psiModule.GetPsiServices().ParallelFinder;
        var baseTypes = new Collection<IDeclaredType>
        {
            KnownTypesFactory.GetByClrTypeName(KnownTypes.ITransientDependency, psiModule),
            KnownTypesFactory.GetByClrTypeName(KnownTypes.ISingletonDependency, psiModule),
            KnownTypesFactory.GetByClrTypeName(KnownTypes.IScopedDependency, psiModule)
        };

        var inheritors = new Collection<IClass>();
        foreach (var baseType in baseTypes)
        {
            finder.FindInheritors(
                baseType.GetTypeElement()!,
                inheritors.ConsumeFilteredDeclaredElements(c => c.Module.Equals(psiModule)),
                progressIndicator);
        }

        foreach (var inheritor in inheritors)
        {
            if (TryGet(inheritor, out var injectable))
                yield return injectable;
        }
    }

    public override bool TryGet(ITypeElement type, [NotNullWhen(true)] out InjectableService? injectable)
    {
        injectable = null;
        if (type.DerivesFrom(KnownTypes.ITransientDependency))
        {
            injectable = new InjectableService([type], type, InjectableServiceLifetime.Transient);
            return true;
        }

        if (type.DerivesFrom(KnownTypes.ISingletonDependency))
        {
            injectable = new InjectableService([type], type, InjectableServiceLifetime.Singleton);
            return true;
        }

        if (type.DerivesFrom(KnownTypes.IScopedDependency))
        {
            injectable = new InjectableService([type], type, InjectableServiceLifetime.Scoped);
            return true;
        }

        return false;
    }
}