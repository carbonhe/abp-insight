using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;

public class Clazz
{
    public void Method1()
    {
        IEventBus eb = NullLocalEventBus.Instance;
        eb.PublishAsync(new Clazz());
    }
}