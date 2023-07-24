// <copyright file="WeakEvent.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;

public sealed class WeakEvent : WeakEventBase, IWeakEvent
{
    public Subscription Subscribe(Action action)
    {
        return SubscribeInternal(() => action);
    }

    public Subscription Subscribe<T>(Func<T> action)
    {
        return SubscribeInternal(action);
    }

    public void Publish()
    {
        PublishInternal<object>(null);
    }
}