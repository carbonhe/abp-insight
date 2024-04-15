using System.Collections.Generic;
using System.Linq;
using JetBrains;
using JetBrains.Application.changes;
using JetBrains.Collections;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Utils;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Assemblies.Impl;
using JetBrains.ProjectModel.Tasks;
using JetBrains.Util;
using JetBrains.Util.Reflection;

namespace AbpInsight.ProjectModel;

public interface IAbpReferenceChangeHandler
{
    void OnHasAbpReference();

    void OnAbpProjectAdded(Lifetime projectLifetime, IProject project);
}

[SolutionComponent]
public class AbpReferencesTracker : IChangeProvider
{
    public static readonly Key<AbpReferencesTracker> AbpReferencesTrackerKey = new(nameof(AbpReferencesTrackerKey));

    private static readonly JetHashSet<string> AbpReferenceNames = ["Volo.Abp.Core"];


    private static readonly ICollection<AssemblyNameInfo> AbpReferenceNameInfos;

    private readonly Lifetime _lifetime;
    private readonly ILogger _logger;
    private readonly ISolution _solution;
    private readonly ModuleReferenceResolveSync _moduleReferenceResolveSync;
    private readonly ChangeManager _changeManager;
    private readonly IViewableProjectsCollection _projects;
    private readonly ICollection<IAbpReferenceChangeHandler> _handlers;
    private readonly Dictionary<IProject, Lifetime> _allProjectLifetimes;
    private readonly HashSet<IProject> _abpProjects;

    private bool _hasAbpReference;

    static AbpReferencesTracker()
    {
        AbpReferenceNameInfos = new List<AssemblyNameInfo>();
        foreach (var name in AbpReferenceNames)
        {
            AbpReferenceNameInfos.Add(AssemblyNameInfoFactory.Create2(name, null));
        }
    }


    public AbpReferencesTracker(
        Lifetime lifetime,
        IEnumerable<IAbpReferenceChangeHandler> handlers,
        ISolution solution,
        ISolutionLoadTasksScheduler scheduler,
        ModuleReferenceResolveSync moduleReferenceResolveSync,
        ChangeManager changeManager,
        IViewableProjectsCollection projects,
        ILogger logger)
    {
        _allProjectLifetimes = new Dictionary<IProject, Lifetime>();
        _abpProjects = [];

        _lifetime = lifetime;
        _handlers = handlers.ToList();
        _solution = solution;
        _moduleReferenceResolveSync = moduleReferenceResolveSync;
        _changeManager = changeManager;
        _projects = projects;
        _logger = logger;


        scheduler.EnqueueTask(new SolutionLoadTask(GetType(), "Preparing Abp Project", SolutionLoadTaskKinds.PreparePsiModules,
            OnSolutionPreparePsiModules));
    }

    private void OnSolutionPreparePsiModules()
    {
        _changeManager.RegisterChangeProvider(_lifetime, this);
        _changeManager.AddDependency(_lifetime, this, _moduleReferenceResolveSync);

        _projects.Projects.View(_lifetime, (lifetime, project) =>
        {
            project.PutData(AbpReferencesTrackerKey, this);
            _allProjectLifetimes.Add(lifetime, project, lifetime);

            if (HasAbpReferences(project))
            {
                _abpProjects.Add(lifetime, project);
            }

            var abpProjectLifetimes = _allProjectLifetimes.Where(it => HasAbpReferences(it.Key)).ToList();

            if (abpProjectLifetimes.Count == 0)
            {
                return;
            }

            NotifyHasAbpReference();
            NotifyOnAbpProjectAdded(abpProjectLifetimes);
        });
    }


    public bool IsAbpProject(IProject? project)
    {
        return project != null && project.IsValid() && _abpProjects.Contains(project);
    }


    private void NotifyHasAbpReference()
    {
        if (!_hasAbpReference)
        {
            _hasAbpReference = true;
            foreach (var handler in _handlers)
            {
                handler.OnHasAbpReference();
            }
        }
    }


    private void NotifyOnAbpProjectAdded(List<KeyValuePair<IProject, Lifetime>> abpProjectLifetimes)
    {
        foreach (var handler in _handlers)
        {
            foreach (var (project, lifetime) in abpProjectLifetimes)
            {
                handler.OnAbpProjectAdded(lifetime, project);
            }
        }
    }

    public object? Execute(IChangeMap changeMap)
    {
        var projectModelChange = changeMap.GetChange<ProjectModelChange>(_solution);
        if (projectModelChange == null)
        {
            return null;
        }

        var changes = ReferencedAssembliesService.TryGetAssemblyReferenceChanges(projectModelChange, AbpReferenceNameInfos, _logger.WhenTrace());
        var newAbpProjects = new List<KeyValuePair<IProject, Lifetime>>();
        foreach (var change in changes)
        {
            if (change.IsAdded)
            {
                var project = change.GetNewProject();
                if (HasAbpReferences(project))
                {
                    Assertion.Assert(_allProjectLifetimes.ContainsKey(project), "project is not added");
                    if (_allProjectLifetimes.TryGetValue(project, out var projectLifetime))
                    {
                        newAbpProjects.Add(JetKeyValuePair.Of(project, projectLifetime));
                        if (!_abpProjects.Contains(project))
                        {
                            _abpProjects.Add(projectLifetime, project);
                        }
                    }
                }
            }
        }

        if (newAbpProjects.Count > 0)
        {
            _changeManager.ExecuteAfterChange(() =>
            {
                NotifyHasAbpReference();
                NotifyOnAbpProjectAdded(newAbpProjects);
            });
        }

        return null;
    }

    private static bool HasAbpReferences(IProject project)
    {
        var targetFrameworkId = project.GetCurrentTargetFrameworkId();
        return project.GetModuleReferences(targetFrameworkId).Any(reference => AbpReferenceNames.Contains(reference.Name));
    }
}