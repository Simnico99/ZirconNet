using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public sealed class WeakEvent : WeakEventBase, IWeakEvent
{
    private static readonly byte _internalByte = new();

    public Subscription Subscribe(Action action)
    {
        return SubscribeInternal<byte>((_) => action());
    }

    public Task PublishAsync()
    {
        return PublishInternalAsync(_internalByte);
    }

    public void Publish()
    {
        PublishInternal(_internalByte);
    }
}