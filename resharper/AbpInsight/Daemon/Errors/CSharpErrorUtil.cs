using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Stages;

namespace AbpInsight.Daemon.Errors;

public static class CSharpErrorUtil
{
    public static DocumentRange InternalRange(string method, params object[] parameters)
    {
        return (DocumentRange)typeof(CSharpErrorStage).Assembly
            .GetType("JetBrains.ReSharper.Daemon.CSharp.Stages.CSharpErrorUtil")
            .GetMethod(method, parameters.Select(it => it.GetType()).ToArray())!
            .Invoke(null, parameters);
    }
}