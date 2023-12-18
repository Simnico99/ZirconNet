﻿// <copyright file="WeakEventT.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Events;

public sealed class WeakEvent<T> : WeakEventBase<T>, IWeakEvent<T>
{
    public void Subscribe(Action<T> action)
    {
        SubscribeInternal(action);
    }

    public void Subscribe(Func<T> action)
    {
        SubscribeInternal(action);
    }

    public void Unsubscribe(Action<T> action)
    {
        UnsubscribeInternal(action);
    }

    public void Unsubscribe(Func<T> action)
    {
        UnsubscribeInternal(action);
    }

    public void Publish(T data)
    {
        PublishInternal(data);
    }
}