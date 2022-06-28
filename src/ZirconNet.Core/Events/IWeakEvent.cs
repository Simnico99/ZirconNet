namespace ZirconNet.Core.Events;
public interface IWeakEvent<T>
{
    void Publish(T data);
    Subscription Subscribe(Action<T> action);
}