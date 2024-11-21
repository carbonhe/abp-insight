using AbpInsight.Utils;
using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace AbpInsight.VoloAbp;

[SolutionComponent(Instantiation.DemandAnyThreadSafe)]
public class AbpInsighter
{
    public static bool IsAbpModuleType(ITypeElement? typeElement)
    {
        return typeElement is IClass { IsAbstract: false } clazz && !clazz.HasTypeParameters() && clazz.DerivesFrom(KnownTypes.IAbpModule);
    }
}