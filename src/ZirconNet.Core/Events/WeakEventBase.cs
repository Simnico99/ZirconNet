// <copyright file="WeakEventBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Collections.Concurrent;
using ZirconNet.Core.Async;

namespace ZirconNet.Core.Events;

public abstract class WeakEventBase<T>
{
    private readonly ConcurrentDictionary<Delegate, WeakReference<Delegate>> _handlers = new();
    private readonly bool _isAsync;

    public WeakEventBase(bool isAsync = false)
    {
        _isAsync = isAsync;
    }

    public void SubscribeInternal<TDelegate>(TDelegate handler)
        where TDelegate : Delegate
    {
        _handlers[handler] = new WeakReference<Delegate>(handler);
    }

    public bool UnsubscribeInternal<TDelegate>(TDelegate handler)
        where TDelegate : Delegate
    {
        return _handlers.TryRemove(handler, out var _);
    }

    public void PublishInternal(T data)
    {
        if (_isAsync)
        {
            Parallel.ForEach(_handlers, handler =>
            {
                if (handler.Value.TryGetTarget(out var target))
                {
                    ExecuteTarget(data, target);
                }
            });
        }
        else
        {
            foreach (var handler in _handlers)
            {
                if (handler.Value.TryGetTarget(out var target))
                {
                    ExecuteTarget(data, target);
                }
            }
        }
    }

    private static void ExecuteTarget(T data, Delegate target)
    {
        switch (target)
        {
            case Action<T> action:
                action(data);
                break;
            case Func<T> func:
                func();
                break;
        }
    }
}
