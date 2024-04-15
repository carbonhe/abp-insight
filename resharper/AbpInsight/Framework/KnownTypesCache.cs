﻿using System.Collections.Concurrent;
using JetBrains.Application.Parts;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace AbpInsight.Framework;

[SolutionComponent(Instantiation.DemandAnyThread)]
public class KnownTypesCache
{
    private readonly ConcurrentDictionary<IClrTypeName, IDeclaredType> _types = new();

    public IDeclaredType GetByClrTypeName(IClrTypeName typeName, IPsiModule module)
    {
        const NullableAnnotation nullableAnnotation = NullableAnnotation.Unknown;

        var type = module.GetPredefinedType().TryGetType(typeName, nullableAnnotation);
        if (type != null)
        {
            return type;
        }

        // Make sure the type is still valid before handing it out. It might be invalid if the module used to create
        // it has been changed
        type = _types.AddOrUpdate(typeName, name => TypeFactory.CreateTypeByCLRName(name, nullableAnnotation, module),
            (name, existingValue) => existingValue.Module.IsValid()
                ? existingValue
                : TypeFactory.CreateTypeByCLRName(name, nullableAnnotation, module));
        return type;
    }
}

public static class KnownTypesFactory
{
    public static IDeclaredType GetByClrTypeName(IClrTypeName typeName, IPsiModule module)
    {
        var knownTypesCache = module.GetSolution().GetComponent<KnownTypesCache>();
        return knownTypesCache.GetByClrTypeName(typeName, module);
    }
}