// <copyright file="AsyncTaskQueue.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Collections.Concurrent;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Async;

public sealed class AsyncTaskQueue
{
    private readonly ConcurrentBag<Exception> _exceptions = new();

    private SemaphoreSlim _taskSemaphore;
    private SemaphoreSlim _queueSemaphore;
    private SemaphoreSlim _waitForFirst;
    private int _tasksInQueue = 0;
    private int _maximumThreads = -1;

    public AsyncTaskQueue(int maximumThreads = -1)
    {
        SetMaxThreads(maximumThreads);
        _taskSemaphore = new SemaphoreSlim(_maximumThreads);
        _queueSemaphore = new SemaphoreSlim(0, int.MaxValue);
        _waitForFirst = new SemaphoreSlim(0, int.MaxValue);
    }

    public bool IsFaulted { get; private set; } = false;

    public async Task EnqueueAsync(Func<Task> actionToRun, CancellationToken cancellationToken = default) => await RunAction(actionToRun, cancellationToken).ConfigureAwait(false);

    public async Task EnqueueAsync(Func<ValueTask> actionToRun, CancellationToken cancellationToken = default) => await RunAction(actionToRun, cancellationToken).ConfigureAwait(false);

    public async ValueTask WaitForQueueToEnd(bool waitForFirstTask = true, CancellationToken cancellationToken = default)
    {
        if (waitForFirstTask)
        {
            await _waitForFirst.WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        if (_tasksInQueue > 0)
        {
            await _queueSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public void Reset(int maximumThreads = -1)
    {
        SetMaxThreads(maximumThreads);

        _taskSemaphore = new SemaphoreSlim(_maximumThreads);
        _queueSemaphore = new SemaphoreSlim(0, int.MaxValue);
        _waitForFirst = new SemaphoreSlim(0, int.MaxValue);

        _tasksInQueue = 0;
        IsFaulted = false;
    }

    private async Task RunAction(Func<Task> actionToRun, CancellationToken cancellationToken)
    {
        await RunActionCore(async () => await actionToRun().ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
    }

    private async Task RunAction(Func<ValueTask> actionToRun, CancellationToken cancellationToken)
    {
        await RunActionCore(async () => await actionToRun().ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
    }

    private async Task RunActionCore(Func<Task> actionToRun, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _tasksInQueue);
        await _taskSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        if (!cancellationToken.IsCancellationRequested)
        {
            ThreadPool.QueueUserWorkItem(_ => RunTask(actionToRun).Forget());
        }
    }

    private async Task RunTask(Func<Task> actionToRun)
    {
        try
        {
            _waitForFirst.Release();
            await actionToRun().ConfigureAwait(false);
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

    private void SetMaxThreads(int maximumThreads)
    {
        if (maximumThreads <= 0 || maximumThreads > Environment.ProcessorCount)
        {
            _maximumThreads = Environment.ProcessorCount;
        }
    }
}
