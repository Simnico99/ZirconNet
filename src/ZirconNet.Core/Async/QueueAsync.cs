using Microsoft.Extensions.Logging;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Async;
public sealed class QueueAsync
{
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly SemaphoreSlim _lockSemaphore = new(1, 1);
    private readonly IList<Func<Task>> _queuedActions = new List<Func<Task>>();
    private readonly ILogger? _logger;

    public QueueAsync(int maximumThreads = -1, ILogger? logger = null)
    {
        if (maximumThreads <= 0)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _semaphoreSlim = new(maximumThreads);
        _logger = logger;
    }

    private async Task RunAction(Func<Task> actionToRun)
    {
        if (_queuedActions.CountThreadSafe() == 0)
        {
            await _lockSemaphore.WaitAsync();
        }

        _queuedActions.AddThreadSafe(actionToRun);
        await _semaphoreSlim.WaitAsync();

        _ = Task.Run(async () =>
        {
            try
            {
                await actionToRun.Invoke();
            }
            finally
            {
                _ = _semaphoreSlim.Release();
                _queuedActions.RemoveThreadSafe(actionToRun);

                if (_queuedActions.CountThreadSafe() == 0)
                {
                    _ = _lockSemaphore.Release();
                }
            }
        });
    }

    /// <summary>
    /// Add a task to the running queue.
    /// </summary>
    /// <param name="actionToRun">The current task to run.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current running action</returns>
    public async Task AddTaskAsync(Func<Task> actionToRun, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await RunAction(actionToRun);
        }
    }

    /// <summary>
    /// Wait for the current queued items to reach 0.
    /// </summary>
    /// <returns>The current task that wait the queue to end.</returns>
    /// <exception cref="SemaphoreFullException"></exception>
    public async Task WaitForQueueEnd()
    {
        try
        {
            await _lockSemaphore.WaitAsync();
            _ = _lockSemaphore.Release();
        }
        catch (SemaphoreFullException e)
        {
            _logger?.LogWarning(e, "Error when waiting for semaphoreslim to end");
        }
    }
}