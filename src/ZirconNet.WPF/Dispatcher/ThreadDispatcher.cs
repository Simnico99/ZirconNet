using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;
public sealed class ThreadDispatcher
{
    private static readonly ThreadDispatcher _instance = new();
    public static ThreadDispatcher Current => _instance;

    private readonly int _mainThreadId;
    private readonly System.Windows.Threading.Dispatcher _dispatcher;
    
    private ThreadDispatcher()
    {
        _mainThreadId = Application.Current.Dispatcher.Thread.ManagedThreadId;
        _dispatcher = Application.Current.Dispatcher;
    }

    public void Invoke(Action action)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            action();
            return;
        }

        _dispatcher.Invoke(action);
    }

    public async ValueTask InvokeAsync(Action action)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            action();
            return;
        }

        await _dispatcher.InvokeAsync(action);
    }

    public async ValueTask<T> InvokeAsync<T>(Func<T> func)
    {
        if (_mainThreadId == Environment.CurrentManagedThreadId)
        {
            return func();
        }

        return await _dispatcher.InvokeAsync(func);
    }
}