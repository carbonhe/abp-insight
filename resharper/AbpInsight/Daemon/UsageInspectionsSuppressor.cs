using System;
using AbpInsight.Framework;
using AbpInsight.ProjectModel;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.Parts;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Psi;

namespace AbpInsight.Daemon;

[ShellComponent(Instantiation.DemandAnyThreadSafe)]
public class UsageInspectionsSuppressor : IUsageInspectionsSuppressor
{
    public bool SuppressUsageInspectionsOnElement(IDeclaredElement element, out ImplicitUseKindFlags flags)
    {
        flags = ImplicitUseKindFlags.Default;
        if (!element.GetSolution().HasAbpReference())
            return false;

        switch (element)
        {
            case IClass clazz when AbpInsighter.IsAbpModuleType(clazz):
                flags = ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature;
                return true;
        }

        return false;
    }
}