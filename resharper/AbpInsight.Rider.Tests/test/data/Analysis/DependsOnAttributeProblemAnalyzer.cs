using Volo.Abp.Modularity;

// I don't know why only the Test02 mark as error in gold, it's work as expected in real IDE envoriment.
[DependsOn(typeof(int),typeof(string),typeof(Test02),typeof(Test03Module))]
public class Test01Module : AbpModule
{
    
}

public class Test02
{
    
}

public class Test03Module : AbpModule
{
    
}