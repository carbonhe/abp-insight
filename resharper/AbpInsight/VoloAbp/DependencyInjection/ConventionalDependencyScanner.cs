using System;
using System.Collections.Generic;
using System.Linq;
using AbpInsight.Utils;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using Microsoft.Extensions.DependencyInjection;

namespace AbpInsight.VoloAbp.DependencyInjection;

public static class ConventionalDependencyScanner
{
    public static IEnumerable<DependencyDescriptor> Scan(IPsiModule psiModule)
    {
        foreach (var typeElement in psiModule.GetPsiServices().Symbols.GetSymbolScope(psiModule, false, true).GetAllTypeElementsGroupedByName())
        {
            if (typeElement is IClass clazz)
            {
                var dependencyDescriptor = Scan(clazz);
                if (dependencyDescriptor != null)
                    yield return dependencyDescriptor;
            }
        }
    }

    public static DependencyDescriptor? Scan(IClass clazz)
    {
        if (clazz.IsAbstract || clazz.HasTypeParameters())
            return null;


        // check DependencyAttribute
        if (clazz.HasAttributeInstance(KnownTypes.DisableConventionalRegistrationAttribute, AttributesSource.Self | AttributesSource.Inherited))
            return null;

        ServiceLifetime? lifetime = null;
        var tryRegister = true;
        var replaceServices = false;

        var attributes = clazz.GetAttributeInstances(KnownTypes.DependencyAttribute, AttributesSource.Self | AttributesSource.Inherited);

        if (attributes.Count == 1)
            (lifetime, tryRegister, replaceServices) = AnalyzeDependencyAttribute(attributes[0]);


        if (!tryRegister)
            return null;

        if (lifetime == null)
        {
            // check class hierarchy
            if (clazz.DerivesFrom(KnownTypes.ITransientDependency))
            {
                lifetime = ServiceLifetime.Transient;
            }
            else if (clazz.DerivesFrom(KnownTypes.ISingletonDependency))
            {
                lifetime = ServiceLifetime.Singleton;
            }
            else if (clazz.DerivesFrom(KnownTypes.IScopedDependency))
            {
                lifetime = ServiceLifetime.Scoped;
            }
        }

        if (lifetime == null)
            return null;

        attributes = clazz.GetAttributeInstances(KnownTypes.ExposeServicesAttribute, AttributesSource.Self | AttributesSource.Inherited);

        var serviceTypes = AnalyzeExposeServicesAttributes(clazz, attributes);

        return new DependencyDescriptor(clazz, lifetime.Value, serviceTypes.ToArray());
    }

    private static (ServiceLifetime?, bool, bool) AnalyzeDependencyAttribute(IAttributeInstance attribute)
    {
        ServiceLifetime? lifetime = null;

        var attributeValue = attribute.PositionParameters().FirstOrDefault() ?? attribute.NamedParameter("Lifetime");
        if (
            attributeValue.IsConstant &&
            attributeValue.ConstantValue.Type.GetTypeElement()?.GetClrName().FullName == KnownTypes.ServiceLifetime.FullName
        )
        {
            if (Enum.TryParse<ServiceLifetime>(attributeValue.ConstantValue.IntValue.ToString(), out var lt))
                lifetime = lt;
        }

        var tryRegister = attribute.NamedParameter("TryRegister").TryGetBoolean();

        var replaceServices = attribute.NamedParameter("ReplaceServices").TryGetBoolean();

        return (lifetime, tryRegister, replaceServices);
    }

    private static IEnumerable<ITypeElement> AnalyzeExposeServicesAttributes(IClass clazz, IList<IAttributeInstance> attributes)
    {
        var resolvedAttributes = attributes.Select(it =>
        {
            var includeDefaults = it.NamedParameter("IncludeDefaults").TryGetBoolean();
            var includeSelf = it.NamedParameter("IncludeSelf").TryGetBoolean();
            var serviceTypes = new List<ITypeElement>();
            var attributeValue = it.PositionParameters().FirstOrDefault();
            if (attributeValue?.IsArray == true)
            {
                serviceTypes.AddRange(
                    (from value in attributeValue.ArrayValue where value.IsType select value.TypeValue.GetTypeElement()).Where(t => t != null));
            }

            return (includeDefaults, includeSelf, serviceTypes.ToArray());
        }).DefaultIfEmpty((true, true, EmptyArray<ITypeElement>.Instance));

        foreach (var (includeDefaults, includeSelf, serviceTypes) in resolvedAttributes)
        {
            foreach (var serviceType in serviceTypes)
            {
                yield return serviceType;
            }

            if (includeSelf)
                yield return clazz;

            if (includeDefaults)
            {
                foreach (var interfaceType in clazz.GetAllInterfaces())
                {
                    var interfaceName = interfaceType.ShortName;

                    if (interfaceType.HasTypeParameters())
                    {
                        var i = interfaceName.IndexOf("`", StringComparison.Ordinal);
                        interfaceName = interfaceName[..i];
                    }

                    if (interfaceName.StartsWith("I", StringComparison.Ordinal))
                        interfaceName = interfaceName[1..];

                    if (clazz.ShortName.EndsWith(interfaceName, StringComparison.OrdinalIgnoreCase))
                        yield return interfaceType;
                }
            }
        }
    }
}

public record DependencyDescriptor(IClass Implementation, ServiceLifetime Lifetime, ITypeElement[] Services)
{
}