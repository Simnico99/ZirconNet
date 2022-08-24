using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;

public interface IWeakEvent<T>
{
    void Publish(T data);
    ConfiguredTaskAwaitable PublishAsync(T data, bool awaitAllCalls = false);
    Subscription Subscribe(Action<T> action);
}