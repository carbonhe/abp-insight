using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AbpInsight.VoloAbp;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;

namespace AbpInsight.Utils;

public static class DeclaredElementExtensions
{
    public static bool DerivesFrom([NotNullWhen(true)] this ITypeElement? typeElement, IClrTypeName baseTypeName)
    {
        if (typeElement == null)
            return false;
        var knownTypesCache = typeElement.GetSolution().GetComponent<KnownTypesCache>();
        var baseType = GetTypeElement(baseTypeName, knownTypesCache, typeElement.Module);
        return typeElement.IsDescendantOf(baseType);
    }

    public static IEnumerable<IInterface> GetAllInterfaces(this ITypeElement typeElement)
    {
        return from superType in typeElement.GetAllSuperTypes() where superType.IsInterfaceType() select superType.GetInterfaceType()!;
    }

    private static ITypeElement? GetTypeElement(IClrTypeName typeName, KnownTypesCache knownTypesCache, IPsiModule module)
    {
        using (CompilationContextCookie.GetExplicitUniversalContextIfNotSet())
        {
            var type = knownTypesCache.GetByClrTypeName(typeName, module);
            return type.GetTypeElement();
        }
    }
}