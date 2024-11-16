using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Application.Parts;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;

namespace AbpInsight.Framework.Injectable;

[SolutionComponent(Instantiation.DemandAnyThreadSafe)]
public class AttributedInjectableServiceProvider : InjectableServiceProviderBase
{
    public override IEnumerable<InjectableService> Scan(IPsiModule psiModule, IProgressIndicator pi)
    {
        foreach (var typeElement in psiModule.GetPsiServices().Symbols.GetSymbolScope(psiModule, false, true).GetAllTypeElementsGroupedByName())
        {
            if (TryGet(typeElement, out var injectable))
            {
                yield return injectable;
            }
        }
    }

    public override bool TryGet(ITypeElement type, [NotNullWhen(true)] out InjectableService? injectable)
    {
        injectable = null;

        var attributes = type.GetAttributeInstances(KnownTypes.DependencyAttribute, AttributesSource.Self | AttributesSource.Inherited);

        if (Enumerable.Any(attributes))
        {
            var dependency = new Dependency(attributes[0]);

            if (dependency.Lifetime.HasValue)
            {
                injectable = new InjectableService([type], type, dependency.Lifetime.Value);
                return true;
            }
        }

        return false;
    }


    private class Dependency
    {
        public InjectableServiceLifetime? Lifetime { get; }

        public bool TryRegister { get; set; }

        public bool ReplaceServices { get; set; }

        public Dependency(IAttributeInstance attribute)
        {
            var attributeValue = attribute.PositionParameters().FirstOrDefault();
            if (attributeValue?.IsConstant == true
                && KnownTypes.ServiceLifetime.FullName == attributeValue.ConstantValue.Type.GetTypeElement()?.GetClrName().FullName)
            {
                Lifetime = (InjectableServiceLifetime)attributeValue.ConstantValue.IntValue;
            }

            attributeValue = attribute.NamedParameter("Lifetime");
            if (attributeValue.IsConstant)
                Lifetime = (InjectableServiceLifetime)attributeValue.ConstantValue.IntValue;

            attributeValue = attribute.NamedParameter("TryRegister");
            TryRegister = attributeValue.TryGetBoolean();

            attributeValue = attribute.NamedParameter("ReplaceServices");
            ReplaceServices = attributeValue.TryGetBoolean();
        }
    }
}