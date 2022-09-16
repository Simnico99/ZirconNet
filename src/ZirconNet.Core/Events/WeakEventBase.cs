using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ZirconNet.Core.Events;
public abstract class WeakEventBase
{
    protected readonly object _locker = new();
    protected readonly List<(Type EventType, Delegate MethodToCall)> _eventRegistrations = new();

    protected virtual Subscription SubscribeInternal<T>(Action<T> action)
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
                _ = _eventRegistrations.Remove((typeof(T), action));
            }
        });
    }

    protected virtual ConfiguredTaskAwaitable PublishInternalAsync<T>(T data, bool configureAwait = false)
    {
        return Task.Run(() =>
        {
            lock (_locker)
            {
#if NET5_0_OR_GREATER
                foreach (var (EventType, MethodToCall) in CollectionsMarshal.AsSpan(_eventRegistrations))
                {
                    if (EventType == typeof(T))
                    {
                        ((Action<T>)MethodToCall)(data);
                    }
                }
#else
                foreach (var (EventType, MethodToCall) in _eventRegistrations)
                {
                    if (EventType == typeof(T))
                    {
                        ((Action<T>)MethodToCall)(data);
                    }
                }
#endif
            }
        }).ConfigureAwait(configureAwait);
    }

    protected virtual void PublishInternal<T>(T data)
    {
        _ = Task.Run(() =>
        {
            lock (_locker)
            {
#if NET5_0_OR_GREATER
                foreach (var (EventType, MethodToCall) in CollectionsMarshal.AsSpan(_eventRegistrations))
                {
                    if (EventType == typeof(T))
                    {
                        ((Action<T>)MethodToCall)(data);
                    }
                }
#else
                foreach (var (EventType, MethodToCall) in _eventRegistrations)
                {
                    if (EventType == typeof(T))
                    {
                        ((Action<T>)MethodToCall)(data);
                    }
                }
#endif
            }
        });
    }
}
