namespace ZirconNet.Core.Events;

public interface IWeakEvent
{
    void Publish();
    Subscription Subscribe(Action action);
}