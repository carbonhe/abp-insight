using System.Diagnostics.CodeAnalysis;
using JetBrains.Metadata.Reader.API;
using JetBrains.Metadata.Reader.Impl;

namespace AbpInsight.VoloAbp;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class KnownTypes
{
    // Modularity
    public static readonly IClrTypeName IAbpModule = new ClrTypeName("Volo.Abp.Modularity.IAbpModule");
    public static readonly IClrTypeName AbpModule = new ClrTypeName("Volo.Abp.Modularity.AbpModule");
    public static readonly IClrTypeName DependsOnAttribute = new ClrTypeName("Volo.Abp.Modularity.DependsOnAttribute");

    // DependencyInjection
    public static readonly IClrTypeName ITransientDependency = new ClrTypeName("Volo.Abp.DependencyInjection.ITransientDependency");
    public static readonly IClrTypeName ISingletonDependency = new ClrTypeName("Volo.Abp.DependencyInjection.ISingletonDependency");
    public static readonly IClrTypeName IScopedDependency = new ClrTypeName("Volo.Abp.DependencyInjection.IScopedDependency");
    public static readonly IClrTypeName DependencyAttribute = new ClrTypeName("Volo.Abp.DependencyInjection.DependencyAttribute");
    public static readonly IClrTypeName ExposeServicesAttribute = new ClrTypeName("Volo.Abp.DependencyInjection.ExposeServicesAttribute");
    public static readonly IClrTypeName ServiceLifetime = new ClrTypeName("Microsoft.Extensions.DependencyInjection.ServiceLifetime");

    public static readonly IClrTypeName DisableConventionalRegistrationAttribute =
        new ClrTypeName("Volo.Abp.DependencyInjection.DisableConventionalRegistrationAttribute");


    // RemoteService
    public static readonly IClrTypeName IRemoteService = new ClrTypeName("Volo.Abp.IRemoteService");
    public static readonly IClrTypeName RemoteServiceAttribute = new ClrTypeName("Volo.Abp.RemoteServiceAttribute");
    public static readonly IClrTypeName IApplicationService = new ClrTypeName("Volo.Abp.Application.Services.IApplicationService");
    public static readonly IClrTypeName ControllerBase = new ClrTypeName("Microsoft.AspNetCore.Mvc.ControllerBase");

    // Event
    public static readonly IClrTypeName IEventHandler = new ClrTypeName("Volo.Abp.EventBus.IEventHandler");

    // DomainService
    public static readonly IClrTypeName IDomainService = new ClrTypeName("Volo.Abp.Domain.Services.IDomainService");

    // Repository
    public static readonly IClrTypeName IRepository = new ClrTypeName("Volo.Abp.Domain.Repositories.IRepository");
}