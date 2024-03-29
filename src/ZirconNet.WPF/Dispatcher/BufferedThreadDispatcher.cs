﻿// <copyright file="BufferedThreadDispatcher.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;

/// <summary>
/// Prevent the ui freeze by using a buffer to execute the UI Updates from multiple threads in a sort of queue. (Neesd to be used with the background service).
/// </summary>
public sealed class BufferedThreadDispatcher
{
    private readonly System.Windows.Threading.Dispatcher _dispatcher;
    private readonly ConcurrentQueue<Action> _queue = new();
    private readonly TaskCompletionSource<object?> _reusableTcs = new();

    private BufferedThreadDispatcher()
    {
        _dispatcher = Application.Current.Dispatcher;
    }

    /// <summary>
    /// Gets the current BufferedThreadDispatcher Instance.
    /// </summary>
    public static BufferedThreadDispatcher Current { get; } = new();

    /// <summary>
    /// Gets or sets delay to wait between the screen refresh.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(1);

    public void Invoke(Action action)
    {
        _queue.Enqueue(action);
    }

    public Task<T>? InvokeAsync<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();
        _queue.Enqueue(() =>
        {
            try
            {
                var result = func();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        return tcs.Task;
    }

    public Task InvokeAsync(Action act)
    {
        _queue.Enqueue(() =>
        {
            try
            {
                act();
                _reusableTcs.SetResult(null);
            }
            catch (Exception ex)
            {
                _reusableTcs.SetException(ex);
            }
        });
        return _reusableTcs.Task;
    }

    /// <summary>
    /// Process one item in the queue.
    /// </summary>
    public void ProcessOneItem()
    {
        if (_queue.TryDequeue(out var action))
        {
            _dispatcher.Invoke(action, DispatcherPriority.Send);
        }
    }
}
