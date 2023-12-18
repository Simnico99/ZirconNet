// <copyright file="WeakEventBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Events;

public abstract class WeakEventBase<T>
{
    private readonly List<WeakReference<Delegate>> _handlers = [];
    private readonly object _syncRoot = new();
    private readonly bool _isAsync;

    public WeakEventBase(bool isAsync = false)
    {
        _isAsync = isAsync;
    }

    public void SubscribeInternal<TDelegate>(TDelegate handler)
        where TDelegate : Delegate
    {
        var weakHandler = new WeakReference<Delegate>(handler);
        lock (_syncRoot)
        {
            _handlers.Add(weakHandler);
        }
    }

    public void UnsubscribeInternal<TDelegate>(TDelegate handler)
        where TDelegate : Delegate
    {
        lock (_syncRoot)
        {
            _handlers.RemoveAll(wr => wr.TryGetTarget(out var existingHandler) && existingHandler.Equals(handler));
        }
    }

    public void PublishInternal(T data)
    {
        var toInvoke = new List<Delegate>();

        lock (_syncRoot)
        {
            _handlers.RemoveAll(wr => !wr.TryGetTarget(out _));
            foreach (var weakReference in _handlers)
            {
                if (weakReference.TryGetTarget(out var handler))
                {
                    toInvoke.Add(handler);
                }
            }
        }

        if (_isAsync)
        {
            foreach (var handler in toInvoke)
            {
                Task.Run(() => InvokeHandler(handler, data)).Forget();
            }
        }
        else
        {
            foreach (var handler in toInvoke)
            {
                InvokeHandler(handler, data);
            }
        }
    }

    private static void InvokeHandler(Delegate handler, T data)
    {
        switch (handler)
        {
            case Action<T> action:
                action(data);
                break;
            case Func<T, object> func:
                func(data);
                break;
        }
    }
}
