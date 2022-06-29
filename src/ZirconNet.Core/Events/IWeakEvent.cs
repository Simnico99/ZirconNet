namespace ZirconNet.Core.Events;
public interface IWeakEvent<T>
{
    Task PublishAsync(T data);
    Subscription Subscribe(Action<T> action);
}