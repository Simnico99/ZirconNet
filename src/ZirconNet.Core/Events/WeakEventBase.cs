using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ZirconNet.Core.Events;
public abstract class WeakEventBase
{
    protected readonly ConcurrentDictionary<Type, List<Delegate>> _eventRegistrations = new();

    protected virtual Subscription SubscribeInternal<T>(Action<T> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _eventRegistrations.AddOrUpdate(typeof(T),
                                       new List<Delegate> { action },
                                       (key, value) => { value.Add(action); return value; });

        return new Subscription(() => _eventRegistrations.AddOrUpdate(typeof(T),
                                       new List<Delegate>(),
                                       (key, value) => { value.Remove(action); return value; }));
    }

    protected virtual void PublishInternal<T>(T? data)
    {
        Task.Run(() => {
            if (_eventRegistrations.TryGetValue(typeof(T), out var actions))
            {
                foreach (var action in actions)
                {
                    ((Action<T?>)action)(data);
                }
            }
        });
    }
}