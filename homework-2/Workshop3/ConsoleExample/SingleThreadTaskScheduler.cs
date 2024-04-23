using System.Collections.Concurrent;

namespace ConsoleExample;

public class SingleThreadTaskScheduler : TaskScheduler, IDisposable
{
    private readonly Thread _thread;
    private readonly ConcurrentQueue<Task> _tasks = new ConcurrentQueue<Task>();
    private ManualResetEventSlim _sync = new ManualResetEventSlim(false);
    private bool _isDisposed;

    public SingleThreadTaskScheduler()
    {
        _thread = new Thread(Run)
        {
            Name = "Lonely Scheduler"
        };
        _thread.Start();
    }

    private void Run()
    {
        while (!_isDisposed)
        {
            _sync.Wait();
            if(_tasks.TryDequeue(out Task? task))
                TryExecuteTask(task);
            else
                _sync.Reset();
        }
    }
    
    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        throw new NotImplementedException();
    }

    protected override void QueueTask(Task task)
    {
        _tasks.Enqueue(task);
        _sync.Set();
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        _isDisposed = true;
        _sync.Set();
        _thread.Join();
    }
}