using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Events;
public interface IWeakEvent<T>
{
    ConfiguredTaskAwaitable PublishAsync(T data);
    Subscription Subscribe(Action<T> action);
}