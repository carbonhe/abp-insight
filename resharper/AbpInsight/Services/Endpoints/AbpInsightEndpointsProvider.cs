using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AbpInsight.VoloAbp;
using AbpInsight.VoloAbp.Mvc;
using JetBrains.Application.changes;
using JetBrains.Application.Parts;
using JetBrains.Application.Progress;
using JetBrains.Application.SynchronizationPoint;
using JetBrains.Application.Threading;
using JetBrains.Application.Threading.AsyncProcessing;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Update;
using JetBrains.ReSharper.Feature.Services.Web.AspRouteTemplates.EndpointsProvider;
using JetBrains.ReSharper.Feature.Services.Web.AspRouteTemplates.EndpointsProvider.AspNetHttpEndpoints;
using JetBrains.ReSharper.Feature.Services.Web.AspRouteTemplates.EndpointsProvider.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.DataFlow;
using JetBrains.Util.Logging;

namespace AbpInsight.Services.Endpoints;

[SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
public class AbpInsightEndpointsProvider : SourceAndModulesChangeConsumer, IHttpEndpointsProvider
{
    private readonly Lifetime _lifetime;
    private readonly ChangeManager _changeManager;
    private readonly IPsiServices _psiServices;
    private readonly AsyncItemsProcessor<IInvalidationScope> _asyncItemsProcessor;

    private readonly ConcurrentDictionary<IPsiModule, AspNetHttpEndpointsRepository> _endpointsPerModuleRepositories;

    public AbpInsightEndpointsProvider(
        Lifetime lifetime,
        ISolution solution,
        ChangeManager changeManager,
        IPsiModules psiModules,
        IPsiServices psiServices,
        AsyncCommitService asyncCommitService,
        SynchronizationPoints synchronizationPoints,
        SolutionDocumentChangeProvider solutionDocumentChangeProvider,
        SuspendHardOperationsManager suspendHardOperationsManager,
        EndpointsSolutionLoadBarrier endpointsSolutionLoadBarrier,
        IShellLocks shellLocks) : base(psiServices, changeManager, solutionDocumentChangeProvider, psiModules, shellLocks, CSharpProjectFileType.Instance)
    {
        _lifetime = lifetime;
        _changeManager = changeManager;
        _psiServices = psiServices;
        var logger = Logger.GetLogger<AbpInsightEndpointsProvider>();
        _endpointsPerModuleRepositories = new ConcurrentDictionary<IPsiModule, AspNetHttpEndpointsRepository>();
        var untilSolutionCloseLifetime = solution.GetSolutionLifetimes().UntilSolutionCloseLifetime;
        _asyncItemsProcessor = AsyncItemsProcessorUtil.CreateWithProcessingOnCommittedPsi<IInvalidationScope>(
                GetType().Name, lifetime, logger, psiServices, asyncCommitService, synchronizationPoints, ProcessScope, InvalidateScope)
            .PauseWhenCachesAreNotReady(lifetime, psiServices)
            .PauseOnSuspendHardOperations(suspendHardOperationsManager)
            .PauseUntilSolutionLoaded(lifetime, endpointsSolutionLoadBarrier);
        IsUpToDate = new Reasons<string>("AbpInsightEndpointsProvider::IsUpToDate", logger)
            .AddWhenFalse(untilSolutionCloseLifetime, _asyncItemsProcessor.ItemsToProcess.IsEmptyNotificationMode,
                () => $"OwnProcessor::IsUpToDate::{Guid.NewGuid()}").AreEmpty;

        untilSolutionCloseLifetime.OnTermination(_endpointsPerModuleRepositories.Clear);
    }

    public string Name => "AbpInsightEndpoints";

    public IProperty<bool> IsUpToDate { get; }


    protected override HashSet<IPsiModule> GetCalculatedModules()
    {
        return _endpointsPerModuleRepositories.Keys.ToHashSet();
    }

    protected override void InvalidateScopes(IEnumerable<IInvalidationScope> scopes)
    {
        foreach (var invalidationScope in scopes)
        {
            InvalidateScope(invalidationScope);
        }
    }

    protected override void ProcessRemovedModules(HashSet<IPsiModule> removedModules)
    {
        foreach (var psiModule in removedModules)
            _endpointsPerModuleRepositories.TryRemove(psiModule, out _);
    }


    protected override int FilesPerModuleCountThreshold => 5;

    IEndpointsTreeNode IEndpointsProvider.GetEndpointsTreeRoot(IPsiModule psiModule)
    {
        return GetEndpointsTreeRoot(psiModule);
    }

    public IReadOnlyCollection<IHttpEndpointsTreeNode> GetEndpointsTreeRoots()
    {
        return _endpointsPerModuleRepositories.Values.Select(it => it.GetEndpointsTreeRoot()).ToArray();
    }

    public IHttpEndpointsTreeNode GetEndpointsTreeRoot(IPsiModule psiModule)
    {
        return GetOrCreateEndpointsPerModuleRepository(psiModule).GetEndpointsTreeRoot();
    }

    IReadOnlyCollection<IEndpointsTreeNode> IEndpointsProvider.GetEndpointsTreeRoots()
    {
        return GetEndpointsTreeRoots();
    }

    private AspNetHttpEndpointsRepository GetOrCreateEndpointsPerModuleRepository(IPsiModule psiModule)
    {
        return _endpointsPerModuleRepositories.GetOrAdd(psiModule, it => new AspNetHttpEndpointsRepository(_lifetime.CreateNested().Lifetime, it));
    }


    private void InvalidateScope(IInvalidationScope invalidationScope)
    {
        _asyncItemsProcessor.ItemsToProcess.Add(invalidationScope);
    }

    private void ProcessScope(IInvalidationScope invalidationScope)
    {
        var psiModule = invalidationScope.Module;
        if (!psiModule.IsValid())
            return;

        _endpointsPerModuleRepositories.TryRemove(psiModule, out _);

        using (CompilationContextCookie.GetExplicitUniversalContextIfNotSet())
        {
            var typeElement = TypeFactory.CreateTypeByCLRName(KnownTypes.IRemoteService, psiModule).GetTypeElement();

            if (typeElement != null)
            {
                var moduleRepository = GetOrCreateEndpointsPerModuleRepository(psiModule);

                foreach (var clazz in psiModule.GetPsiServices().Symbols
                             .GetSymbolScope(psiModule, false, true).GetAllTypeElementsGroupedByName()
                             .Where(it => it.IsDescendantOf(typeElement)).OfType<IClass>())
                {
                    foreach (var endpoint in ConventionalControllerUtil.GetControllerEndpoints(clazz, psiModule))
                    {
                        moduleRepository.AddEndpoint(endpoint);
                    }
                }
            }
        }

        _psiServices.Locks.ExecuteOrQueueEx(_lifetime, Name + ".DispatchChange",
            () => _changeManager.OnProviderChanged(this, new EndpointsTreeChange(Name), SimpleTaskExecutor.Instance));
    }
}