using ConsoleExample;

using var scheduler = new SingleThreadTaskScheduler();

Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
SynchronizationContext.SetSynchronizationContext(new MySyncContext(scheduler));
var locker = new object();

Monitor.Enter(locker);

await Task.Factory.StartNew(
    () => Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}"),
    CancellationToken.None,
    TaskCreationOptions.None,
    scheduler).ConfigureAwait(false);

Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
// await Task.Run(() =>
// );
// await Task.Run(() =>
// {
//     Console.WriteLine("Task running in the thread pool.");
//     // Ваш код здесь
// });