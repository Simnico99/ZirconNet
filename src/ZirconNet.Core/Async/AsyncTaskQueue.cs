using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;

/// <summary>
/// A class that allows queueing and running tasks asynchronously with a specified maximum number of concurrent tasks.
/// </summary>
public sealed class AsyncTaskQueue
{
    private SemaphoreSlim _taskSemaphore;
    private SemaphoreSlim _queueSemaphore;
    private int _tasksInQueue = 0;
    private readonly ConcurrentBag<Exception> _exceptions;
    private readonly int _maximumThreads;

    /// <summary>
    /// Creates a new AsyncTaskQueue with a specified maximum number of concurrent tasks.
    /// </summary>
    /// <param name="maximumThreads">The maximum number of concurrent tasks. Defaults to the number of available processors.</param>
    public AsyncTaskQueue(int maximumThreads = -1)
    {
        if (maximumThreads <= 0 || maximumThreads > Environment.ProcessorCount)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _maximumThreads = maximumThreads;
        _taskSemaphore = new SemaphoreSlim(maximumThreads);
        _queueSemaphore = new SemaphoreSlim(0, int.MaxValue);
        _exceptions = new ConcurrentBag<Exception>();
    }

    private async ValueTask RunAction(Func<ValueTask> actionToRun, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _tasksInQueue);
        await _taskSemaphore.WaitAsync(cancellationToken);

        try
        {
            ValueTask actionTask = actionToRun();
            if (!actionTask.IsCompleted)
            {
                await actionTask;
            }
        }
        catch (Exception ex)
        {
            _exceptions.Add(ex);
        }
        finally
        {
            _taskSemaphore.Release();
            if (Interlocked.Decrement(ref _tasksInQueue) == 0)
            {
                _queueSemaphore.Release();
            }
        }
    }

    private async ValueTask RunAction(Func<Task> actionToRun, CancellationToken cancellationToken)
    {
        await RunAction(() => new ValueTask(actionToRun()), cancellationToken);
    }


    /// <summary>
    /// Adds a ValueTask to the queue and runs it asynchronously.
    /// </summary>
    public ValueTask AddTaskAsync(Func<ValueTask> actionToRun, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            return RunAction(actionToRun, cancellationToken);
        }

        return default;
    }

    /// <summary>
    /// Adds a Task to the queue and runs it asynchronously.
    /// </summary>
    public ValueTask AddTaskAsync(Func<Task> actionToRun, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            return RunAction(actionToRun, cancellationToken);
        }

        return default;
    }

    /// <summary>
    /// Waits for all tasks in the queue to finish.
    /// </summary>
    public async ValueTask WaitForQueueEnd(CancellationToken cancellationToken = default)
    {
        await _queueSemaphore.WaitAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a read-only list of exceptions that occurred while executing the tasks.
    /// </summary>
    public IReadOnlyList<Exception> GetExceptions()
    {
        return _exceptions.ToList().AsReadOnly();
    }

    /// <summary>
    /// Resets the AsyncTaskQueue to its original state, waiting for all tasks to finish.
    /// </summary>
    public async ValueTask Reset(CancellationToken cancellationToken = default)
    {
        await WaitForQueueEnd(cancellationToken);

        _taskSemaphore.Dispose();
        _queueSemaphore.Dispose();
        _taskSemaphore = new SemaphoreSlim(_maximumThreads);
        _queueSemaphore = new SemaphoreSlim(0, int.MaxValue);

        while (_exceptions.TryTake(out _)) ;
    }
}
