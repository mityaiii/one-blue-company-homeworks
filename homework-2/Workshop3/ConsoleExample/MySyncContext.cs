namespace ConsoleExample;

public class MySyncContext : SynchronizationContext
{
    private readonly SingleThreadTaskScheduler _scheduler;

    public MySyncContext(SingleThreadTaskScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        new Task(() => d(state)).Start(_scheduler);
    }
}