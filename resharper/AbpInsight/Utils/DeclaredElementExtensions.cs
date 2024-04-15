using System.Diagnostics.CodeAnalysis;
using AbpInsight.Framework;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace AbpInsight.Utils;

public static class DeclaredElementExtensions
{
    public static bool DerivesFrom([NotNullWhen(true)] this ITypeElement? candidate, IClrTypeName baseTypeName)
    {
        if (candidate == null)
            return false;
        var knownTypesCache = candidate.GetSolution().GetComponent<KnownTypesCache>();
        var baseType = GetTypeElement(baseTypeName, knownTypesCache, candidate.Module);
        return candidate.IsDescendantOf(baseType);
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