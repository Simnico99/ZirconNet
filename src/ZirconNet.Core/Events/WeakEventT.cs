// <copyright file="WeakEventT.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;

public sealed class WeakEvent<T> : WeakEventBase, IWeakEvent<T>
{
    public Subscription Subscribe(Func<T> action)
    {
        return SubscribeInternal(action);
    }

    public Subscription Subscribe(Action<T> action)
    {
        return SubscribeInternal(action);
    }

    public void Publish(T data)
    {
        PublishInternal(data);
    }
}