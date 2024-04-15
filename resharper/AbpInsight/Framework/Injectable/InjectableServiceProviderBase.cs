using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AbpInsight.Utils;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace AbpInsight.Framework.Injectable;

public abstract class InjectableServiceProviderBase
{
    public abstract IEnumerable<InjectableService> Scan(IPsiModule psiModule, IProgressIndicator pi);

    public abstract bool TryGet(ITypeElement type, [NotNullWhen(true)] out InjectableService? injectable);
}

public record InjectableService(ITypeElement[] Services, ITypeElement Implementation, InjectableServiceLifetime Lifetime)
{
    public InjectableServiceKind Kind
    {
        get
        {
            InjectableServiceKind kind = default;
            if (Implementation.DerivesFrom(KnownTypes.IApplicationService))
            {
                kind |= InjectableServiceKind.ApplicationService;
            }

            if (Implementation.DerivesFrom(KnownTypes.IRepository))
            {
                kind |= InjectableServiceKind.Repository;
            }

            if (Implementation.DerivesFrom(KnownTypes.IDomainService))
            {
                kind |= InjectableServiceKind.DomainService;
            }

            if (Implementation.DerivesFrom(KnownTypes.IEventHandler))
            {
                kind |= InjectableServiceKind.EventHandler;
            }

            return kind == default ? InjectableServiceKind.Regular : kind;
        }
    }
}

[Flags]
public enum InjectableServiceKind
{
    Regular = 1 << 0,
    ApplicationService = 1 << 1,
    Repository = 1 << 2,
    DomainService = 1 << 3,
    EventHandler = 1 << 4
}

public enum InjectableServiceLifetime
{
    Singleton,
    Scoped,
    Transient,
    Unknown,
}