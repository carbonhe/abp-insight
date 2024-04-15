using JetBrains.ProjectModel;

namespace AbpInsight.ProjectModel;

public static class ProjectExtensions
{
    public static bool HasAbpReference(this ISolution solution)
    {
        var tracker = solution.GetComponent<AbpSolutionTracker>();
        return tracker.HasAbpReference.Value;
    }

    public static bool IsAbpProject(this IProject? project)
    {
        if (project == null || !project.IsValid())
        {
            return false;
        }

        var tracker = project.GetData(AbpReferencesTracker.AbpReferencesTrackerKey);
        return tracker != null && tracker.IsAbpProject(project);
    }
}