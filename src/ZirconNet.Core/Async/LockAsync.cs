// <copyright file="LockAsync.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Async;

/// <summary>
/// Create lock from an async Task delegate and returns the result.
/// </summary>
public readonly struct LockAsync
{
    private readonly SemaphoreSlim _semaphore;

    public LockAsync()
    {
        _semaphore = new(1, 1);
    }

    public async Task Lock(Func<Task> worker)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            await worker().ConfigureAwait(false);
        }
        finally
        {
            _ = _semaphore?.Release();
        }
    }

    public async Task<T> Lock<T>(Func<Task<T>> worker)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            return await worker().ConfigureAwait(false);
        }
        finally
        {
            _ = _semaphore?.Release();
        }
    }

    public async Task Lock(Func<ValueTask> worker)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            await worker().ConfigureAwait(false);
        }
        finally
        {
            _ = _semaphore?.Release();
        }
    }

    public Task Lock<T>(Func<T> worker)
        where T : Task
    {
        return Lock(worker);
    }

    public async Task<T> Lock<T>(Func<ValueTask<T>> worker)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            return await worker().ConfigureAwait(false);
        }
        finally
        {
            _ = _semaphore?.Release();
        }
    }
}