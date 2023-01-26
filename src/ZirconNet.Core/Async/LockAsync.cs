namespace ZirconNet.Core.Async;

/// <summary>
/// Create lock from an async Task delegate and returns the result.
/// </summary>
public readonly struct LockAsync 
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public LockAsync(){}

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