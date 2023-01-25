using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public interface IWeakEvent<T>
{
    void Publish(T data);
    Task PublishAsync(T data);
    Subscription Subscribe(Action<T> action);
}