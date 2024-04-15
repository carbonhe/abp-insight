using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace AbpInsight.Services.QuickFixes;

// Most QuickFixes are auto-registered, via [QuickFix] and ctor injection.
// Manual registration allows us to reuse an existing quick fix with a different highlighting.
[ShellComponent]
public class QuickFixRegistrar(Lifetime lifetime) : IQuickFixesProvider
{
    public void Register(IQuickFixesRegistrar table)
    {
    }

    public IEnumerable<Type> Dependencies => Array.Empty<Type>();
}