using Volo.Abp.EventBus;

public class Test
{
    public void Test()
    {
        IEventBus eb = null;
        eb.PublishAsync(new Test());
    }
}