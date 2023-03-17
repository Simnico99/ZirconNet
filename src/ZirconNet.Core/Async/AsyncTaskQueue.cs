using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;

/// <summary>
/// A class that allows queueing and running tasks asynchronously with a specified maximum number of concurrent tasks.
/// </summary>
public sealed class AsyncTaskQueue : IDisposable
{
    private SemaphoreSlim _taskSemaphore;
    private TaskCompletionSource<bool> _queueCompletionSource;
    private int _tasksInQueue = 0;
    private readonly ConcurrentBag<Exception> _exceptions;

    public AsyncTaskQueue(int maximumThreads = -1)
    {
        if (maximumThreads <= 0 || maximumThreads > Environment.ProcessorCount)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _taskSemaphore = new SemaphoreSlim(maximumThreads);
        _queueCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _exceptions = new ConcurrentBag<Exception>();
    }

    private async Task RunAction(Func<Task> actionToRun, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _tasksInQueue);
        await _taskSemaphore.WaitAsync(cancellationToken);

        _ = Task.Run(async () =>
        {
            try
            {
                await actionToRun();
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
                    _queueCompletionSource.TrySetResult(true);
                }
            }
        });
    }

    public Task AddTaskAsync(Func<Task> actionToRun, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            return RunAction(actionToRun, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public async Task WaitForQueueEnd(CancellationToken cancellationToken = default)
    {
        using (cancellationToken.Register(() => _queueCompletionSource.TrySetCanceled()))
        {
            await _queueCompletionSource.Task.ConfigureAwait(false);
        }
    }

    public IReadOnlyList<Exception> GetExceptions()
    {
        return _exceptions.ToList().AsReadOnly();
    }

    public async Task Reset(int maximumThreads = -1, CancellationToken cancellationToken = default)
    {
        await WaitForQueueEnd(cancellationToken);

        if (maximumThreads <= 0 || maximumThreads > Environment.ProcessorCount)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _taskSemaphore.Dispose();
        _taskSemaphore = new SemaphoreSlim(maximumThreads);
        _queueCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        while (_exceptions.TryTake(out _)) { }
    }

    public void Dispose()
    {
        _taskSemaphore.Dispose();
    }
}
