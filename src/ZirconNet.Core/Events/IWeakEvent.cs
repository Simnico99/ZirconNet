using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public interface IWeakEvent
{
    void Publish();
    ConfiguredTaskAwaitable PublishAsync(bool configureAwait = false);
    Subscription Subscribe(Action action);
}