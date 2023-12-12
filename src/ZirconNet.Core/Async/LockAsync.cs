// <copyright file="LockAsync.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Runtime;

namespace ZirconNet.Core.Async;

/// <summary>
/// Create lock from an async Task delegate and returns the result.
/// </summary>
public sealed class LockAsync : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private bool _disposedValue;

    public LockAsync()
    {
        _semaphore = new(1, 1);
    }

    public async Task Lock(Func<Task> taskWorker)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            await taskWorker().ConfigureAwait(false);
        }
        finally
        {
            _semaphore?.Release();
        }
    }

    public async Task<T> Lock<T>(Func<Task<T>> taskWorker)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            return await taskWorker().ConfigureAwait(false);
        }
        finally
        {
            _semaphore?.Release();
        }
    }

    /// <summary>
    /// Acquires a lock and executes the provided asynchronous <see cref="ValueTask"/> operation.
    /// Use <c>Lock(valueTaskWorker: () => { actions... })</c> to specify it is a <see cref="ValueTask"/>.
    /// Do not use the dummy parameter.
    /// </summary>
    /// <remarks>
    /// The method is specifically designed for <see cref="ValueTask"/> operations. The second parameter is a dummy and should not be used.
    /// </remarks>
    /// <param name="valueTaskWorker">
    /// The <see cref="ValueTask"/> worker delegate that represents the asynchronous operation to be performed under the lock.
    /// </param>
    /// <param name="_">
    /// <b>DUMMY PARAMETER - DO NOT USE.</b> This parameter is included for prioritizing the methods.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Is an empty parameter")]
    public async Task Lock(Func<ValueTask> valueTaskWorker, DummyParameter? _ = null)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            await valueTaskWorker().ConfigureAwait(false);
        }
        finally
        {
            _semaphore?.Release();
        }
    }

    public Task Lock<T>(Func<T> taskWorker)
        where T : Task
    {
        return Lock(taskWorker);
    }

    /// <summary>
    /// Acquires a lock and executes the provided asynchronous <see cref="ValueTask"/> operation.
    /// Use <c>Lock(valueTaskWorker: () => { actions... })</c> to specify it is a <see cref="ValueTask"/>.
    /// Do not use the dummy parameter.
    /// </summary>
    /// <remarks>
    /// The method is specifically designed for <see cref="ValueTask"/> operations. The second parameter is a dummy and should not be used.
    /// </remarks>
    /// <param name="valueTaskWorker">
    /// The <see cref="ValueTask"/> worker delegate that represents the asynchronous operation to be performed under the lock.
    /// </param>
    /// <param name="_">
    /// <b>DUMMY PARAMETER - DO NOT USE.</b> This parameter is included for prioritizing the methods.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation.
    /// </returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Is an empty parameter")]
    public async Task<T> Lock<T>(Func<ValueTask<T>> valueTaskWorker, DummyParameter? _ = null)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            return await valueTaskWorker().ConfigureAwait(false);
        }
        finally
        {
            _semaphore?.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _semaphore?.Dispose();
            }

            _disposedValue = true;
        }
    }
}