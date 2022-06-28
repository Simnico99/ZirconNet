namespace ZirconNet.Core.Events;
public class WeakEvent<T> : IWeakEvent<T>
{
    private readonly object _locker = new();
    private readonly List<(Type EventType, Delegate MethodToCall)> _eventRegistrations = new();

    public Subscription Subscribe(Action<T> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _eventRegistrations.Add((typeof(T), action));
        return new Subscription(() =>
        {
            lock (_locker)
            {
                _eventRegistrations.Remove((typeof(T), action));
            }
        });
    }

    public void Publish(T data)
    {
        lock (_locker)
        {
            foreach (var (EventType, MethodToCall) in _eventRegistrations)
            {
                if (EventType == typeof(T))
                {
                    ((Action<T>)MethodToCall)(data);
                }
            }
        }
    }
}
