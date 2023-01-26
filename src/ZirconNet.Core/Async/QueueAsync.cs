using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;

public sealed class QueueAsync
{
    private readonly SemaphoreSlim _taskSemaphore;
    private readonly SemaphoreSlim _queueSemaphore;
    private int _tasksInQueue = 0;

    public QueueAsync(int maximumThreads = -1)
    {
        if (maximumThreads <= 0)
        {
            maximumThreads = Environment.ProcessorCount;
        }

        _taskSemaphore = new SemaphoreSlim(maximumThreads);
        _queueSemaphore = new SemaphoreSlim(0,int.MaxValue);
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
                catch (Exception)
                {
                    // Log or handle the exception as appropriate
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

    public async Task WaitForQueueEnd(CancellationToken cancellationToken = default)
    {
        await _queueSemaphore.WaitAsync(cancellationToken);
    }
}