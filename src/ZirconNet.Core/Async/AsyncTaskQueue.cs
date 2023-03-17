using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;

/// <summary>
/// A class that allows queueing and running tasks asynchronously with a specified maximum number of concurrent tasks.
/// </summary>
public sealed class AsyncTaskQueue
{
    private readonly EventWaitHandle waitHandle = new AutoResetEvent(false);
    private SemaphoreSlim _taskSemaphore;
    private SemaphoreSlim _queueSemaphore;
    private SemaphoreSlim _waitForFirst;
    private int _tasksInQueue = 0;
    private readonly ConcurrentBag<Exception> _exceptions = new();

    public bool IsFaulted { get; private set; } = false;

    public AsyncTaskQueue(int maximumThreads = -1)
    {
        if (maximumThreads <= 0 || maximumThreads <= 0 || maximumThreads > Environment.ProcessorCount)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _taskSemaphore = new SemaphoreSlim(maximumThreads);
        _queueSemaphore = new SemaphoreSlim(0, int.MaxValue);
        _waitForFirst = new SemaphoreSlim(0, int.MaxValue);
    }

    private async Task RunAction(Func<Task> actionToRun, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _tasksInQueue);
        await _taskSemaphore.WaitAsync(cancellationToken);

        _ = Task.Run(async () =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await actionToRun();
                }
                catch (Exception ex)
                {
                    IsFaulted = true;
                    _exceptions.Add(ex);
                }
                finally
                {
                    Interlocked.Decrement(ref _tasksInQueue);
                    if (_tasksInQueue == 0)
                    {
                        _queueSemaphore.Release();
                    }
                    _taskSemaphore.Release();
                }
            }
        }, cancellationToken);
    }

    public async Task AddTaskAsync(Func<Task> actionToRun, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await RunAction(actionToRun, cancellationToken);
        }
    }

    public async ValueTask WaitForQueueToEnd(bool waitForFirstTask = true, CancellationToken cancellationToken = default)
    {
        if (waitForFirstTask)
        {
            await _waitForFirst.WaitAsync(cancellationToken);
        }

        if (_tasksInQueue > 0)
        {
            await _queueSemaphore.WaitAsync(cancellationToken);
        }
    }

    public async ValueTask Reset(int maximumThreads = -1)
    {
        if (_tasksInQueue > 0)
        {
            await WaitForQueueToEnd();
        }

        if (maximumThreads <= 0 || maximumThreads <= 0 || maximumThreads > Environment.ProcessorCount)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _taskSemaphore = new (maximumThreads);
        _queueSemaphore = new (0, int.MaxValue);
        _waitForFirst = new(0, int.MaxValue);
        _tasksInQueue = 0;
        IsFaulted = false;
        waitHandle.Reset();
    }
}
