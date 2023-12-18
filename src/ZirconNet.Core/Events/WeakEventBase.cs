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
        lock (_syncRoot)
        {
            _handlers.Add(new WeakReference<Delegate>(handler));
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
        if (_isAsync)
        {
            foreach (var handler in _handlers)
            {
                Task.Run(() => InvokeHandler(handler, data)).Forget(true);
            }
        }
        else
        {
            foreach (var handler in _handlers)
            {
                InvokeHandler(handler, data);
            }
        }
    }

    private static void InvokeHandler(WeakReference<Delegate> handler, T data)
    {
        if (handler.TryGetTarget(out var target))
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
}
