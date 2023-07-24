// <copyright file="ThreadDispatcher.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Windows;
using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;

public sealed class ThreadDispatcher
{
    private readonly int _mainThreadId;
    private readonly System.Windows.Threading.Dispatcher _dispatcher;

    private ThreadDispatcher()
    {
        _mainThreadId = Application.Current.Dispatcher.Thread.ManagedThreadId;
        _dispatcher = Application.Current.Dispatcher;
    }

    public static ThreadDispatcher Current { get; } = new();

    public void Invoke(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            action();
            return;
        }

        _dispatcher.Invoke(action, dispatcherPriority);
    }

    public async ValueTask InvokeAsync(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            action();
            return;
        }

        await _dispatcher.InvokeAsync(action, dispatcherPriority);
    }

    public async ValueTask<T> InvokeAsync<T>(Func<T> func, DispatcherPriority dispatcherPriority = DispatcherPriority.Send)
    {
        return _mainThreadId == Environment.CurrentManagedThreadId ? func() : await _dispatcher.InvokeAsync(func, dispatcherPriority);
    }
}