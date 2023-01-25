using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public interface IWeakEvent
{
    void Publish();
    Task PublishAsync();
    Subscription Subscribe(Action action);
}