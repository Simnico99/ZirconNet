using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public sealed class WeakEvent<T> : WeakEventBase, IWeakEvent<T>
{
    public Subscription Subscribe(Action<T> action)
    {
        return SubscribeInternal(action);
    }

    public ConfiguredTaskAwaitable PublishAsync(T data, bool configureAwait = false)
    {
        return PublishInternalAsync(data, configureAwait);
    }

    public void Publish(T data)
    {
        PublishAsync(data);
    }
}

public sealed class WeakEvent : WeakEventBase, IWeakEvent
{
    public Subscription Subscribe(Action action)
    {

        return SubscribeInternal<byte>((_) => action());
    }

    public ConfiguredTaskAwaitable PublishAsync(bool configureAwait = false)
    {
        return PublishInternalAsync(new byte(), configureAwait);
    }

    public void Publish()
    {
        PublishAsync();
    }
}