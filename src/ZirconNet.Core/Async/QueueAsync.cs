using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Async;
public sealed class QueueAsync : IDisposable, IAsyncDisposable
{
    private bool _disposed;
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly ConcurrentQueue<Func<Task>> _queuedActions = new();
    private readonly ILogger? _logger;

    public QueueAsync(int? maximumThreads = null, ILogger? logger = null)
    {
        _semaphoreSlim = new SemaphoreSlim(maximumThreads ?? Environment.ProcessorCount);
        _logger = logger;
    }

    private async Task RunAction(Func<Task> actionToRun)
    {
        try
        {
            _queuedActions.Enqueue(actionToRun);
            await _semaphoreSlim.WaitAsync();

            await actionToRun();
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "Error running the queued action");
        }
        finally
        {
            _semaphoreSlim.Release();
            _queuedActions.TryDequeue(out _);
        }
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
            await RunAction(actionToRun);
        }
    }

    /// <summary>
    /// Wait for the current queued items to reach 0.
    /// </summary>
    public async Task WaitForQueueEnd()
    {
        using var semaphore = new SemaphoreSlim(0, _queuedActions.Count);
        try
        {
            await semaphore.WaitAsync();
        }
        catch (SemaphoreFullException e)
        {
            _logger?.LogWarning(e, "Error when waiting for semaphoreslim to end");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _semaphoreSlim.Dispose();
        }

        _disposed = true;
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            await Task.Run(() => _semaphoreSlim.Dispose());
        }

        _disposed = true;
    }

}