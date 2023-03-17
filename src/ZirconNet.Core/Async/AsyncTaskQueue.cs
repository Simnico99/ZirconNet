using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;

public sealed class AsyncTaskQueue
{
    private SemaphoreSlim _taskSemaphore;
    private SemaphoreSlim _queueSemaphore;
    private SemaphoreSlim _waitForFirst;
    private int _tasksInQueue = 0;
    private readonly ConcurrentBag<Exception> _exceptions = new();

    public bool IsFaulted { get; private set; } = false;

    public AsyncTaskQueue(int maximumThreads = -1)
    {
        SetMaxThreads(ref maximumThreads);
        _taskSemaphore = new SemaphoreSlim(maximumThreads);
        _queueSemaphore = new SemaphoreSlim(0, int.MaxValue);
        _waitForFirst = new SemaphoreSlim(0, int.MaxValue);
    }

    private async Task RunAction(Func<Task> actionToRun, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _tasksInQueue);
        await _taskSemaphore.WaitAsync(cancellationToken);

        if (!cancellationToken.IsCancellationRequested)
        {
            ThreadPool.QueueUserWorkItem(_ => RunTask(actionToRun));
        }
    }

    private async void RunTask(Func<Task> actionToRun)
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

    public async Task EnqueueTask(Func<Task> actionToRun, CancellationToken cancellationToken = default)
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
        await WaitForQueueToEnd();

        SetMaxThreads(ref maximumThreads);

        _taskSemaphore.Release(maximumThreads - _taskSemaphore.CurrentCount);

        while (_queueSemaphore.CurrentCount > 0)
        {
            _queueSemaphore.Wait();
        }

        while (_waitForFirst.CurrentCount > 0)
        {
            _waitForFirst.Wait();
        }

        _tasksInQueue = 0;
        IsFaulted = false;
    }

    private void SetMaxThreads(ref int maximumThreads)
    {
        if (maximumThreads <= 0 || maximumThreads > Environment.ProcessorCount)
        {
            maximumThreads = Environment.ProcessorCount;
        }
    }
}