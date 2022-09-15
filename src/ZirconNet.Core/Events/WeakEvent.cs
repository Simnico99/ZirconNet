using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
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