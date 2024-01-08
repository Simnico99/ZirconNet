// <copyright file="WeakEvent.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Events;

public sealed class WeakEvent : WeakEventBase<object?>, IWeakEvent
{
    public WeakEvent(bool isAsync = false)
    : base(isAsync)
    {
    }

    public void Subscribe(Action action)
    {
        SubscribeInternal<Action<object?>>((_) => action());
    }

    public void Subscribe<T>(Func<T> action)
    {
        SubscribeInternal(action);
    }

    public bool Unsubscribe(Action action)
    {
        return UnsubscribeInternal<Action<object?>>((_) => action());
    }

    public bool Unsubscribe<T>(Func<T> action)
    {
        return UnsubscribeInternal(action);
    }

    public void Publish()
    {
        PublishInternal(null);
    }
}