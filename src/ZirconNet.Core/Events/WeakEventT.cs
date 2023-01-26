﻿using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public sealed class WeakEvent<T> : WeakEventBase, IWeakEvent<T>
{
    public Subscription Subscribe(Action<T> action)
    {
        return SubscribeInternal(action);
    }

    public void Publish(T data)
    {
        PublishInternal(data);
    }
}