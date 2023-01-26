using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public sealed class WeakEvent : WeakEventBase, IWeakEvent
{
    public Subscription Subscribe(Action action)
    {
        return SubscribeInternal<object>((o) => action());
    }

    public void Publish()
    {
        PublishInternal<object>(null);
    }
}