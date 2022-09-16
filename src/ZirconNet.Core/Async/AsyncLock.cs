namespace ZirconNet.Core.Async;

public sealed class AsyncLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task Lock(Func<Task> worker)
    {
        await _semaphore.WaitAsync();
        try
        {
            await worker();
        }
        finally
        {
            _ = (_semaphore?.Release());
        }
    }

    public async Task<T> Lock<T>(Func<Task<T>> worker)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await worker();
        }
        finally
        {
            _ = (_semaphore?.Release());
        }
    }
}