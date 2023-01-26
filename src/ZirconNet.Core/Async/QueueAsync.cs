using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;

public sealed class QueueAsync
{
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly ConcurrentQueue<Func<Task>> _queuedActions = new();

    public QueueAsync(int maximumThreads = -1)
    {
        if (maximumThreads <= 0)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _semaphoreSlim = new SemaphoreSlim(maximumThreads);
    }

    private async Task RunAction(Func<Task> actionToRun, CancellationToken cancellationToken)
    {
        _queuedActions.Enqueue(actionToRun);
        await _semaphoreSlim.WaitAsync(cancellationToken);

        _ = Task.Run(async () =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await actionToRun.Invoke();
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Add a task to the running queue.
    /// </summary>
    /// <param name="actionToRun">The current task to run.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task AddTaskAsync(Func<Task> actionToRun, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await RunAction(actionToRun, cancellationToken);
        }
    }

    /// <summary>
    /// Wait for the current queued items to reach 0.
    /// </summary>
    public async Task WaitForQueueEnd()
    {
        while (_queuedActions.TryPeek(out _))
        {
            await Task.Delay(10);
        }
    }
}
