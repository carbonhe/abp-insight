using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

class Test01 : ITransientDependency
{
}


[Dependency]
internal class Test02
{
}


public class Test03 : ITransientDependency, IScopedDependency
{
    
}