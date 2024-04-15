using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.Rd.Base;

namespace AbpInsight.ProjectModel;

[SolutionComponent]
public class AbpSolutionTracker : IAbpReferenceChangeHandler
{
    private readonly ISolution _solution;

    public readonly ViewableProperty<bool> HasAbpReference = new(false);


    public AbpSolutionTracker(ISolution solution)
    {
        _solution = solution;
    }

    public void OnHasAbpReference()
    {
        HasAbpReference.Set(true);
    }

    public void OnAbpProjectAdded(Lifetime projectLifetime, IProject project)
    {
    }
}