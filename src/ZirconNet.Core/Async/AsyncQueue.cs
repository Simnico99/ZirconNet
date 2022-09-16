using Microsoft.Extensions.Logging;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Async;
public sealed class AsyncQueue
{
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly SemaphoreSlim _lockSemaphore = new(1, 1);
    private readonly List<Func<Task>> _queuedActions = new();
    private readonly ILogger? _logger;

    public AsyncQueue(int maximumThreads = 8, ILogger? logger = null)
    {
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

    public async Task AddTaskAsync(Func<Task> actionToRun, CancellationToken cancellationToken = default)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await RunAction(actionToRun);
        }
    }

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