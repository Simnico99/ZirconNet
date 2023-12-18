// <copyright file="WeakEvent.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;

public sealed class WeakEvent : WeakEventBase<object?>, IWeakEvent
{
    public void Subscribe(Action action)
    {
        SubscribeInternal<Action<object?>>((_) => action());
    }

    public void Subscribe<T>(Func<T> action)
    {
        SubscribeInternal(action);
    }

    public void Unsubscribe(Action action)
    {
        UnsubscribeInternal<Action<object?>>((_) => action());
    }

    public void Unsubscribe<T>(Func<T> action)
    {
        UnsubscribeInternal(action);
    }

    public void Publish()
    {
        PublishInternal(null);
    }
}