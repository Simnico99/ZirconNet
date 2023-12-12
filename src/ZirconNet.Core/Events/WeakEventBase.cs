// <copyright file="WeakEventBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Events;

public abstract class WeakEventBase
{
    private readonly List<(Type, Delegate)> _eventRegistrations = new();
    private readonly object _lock = new();

    protected virtual Subscription SubscribeInternal<T>(T action)
        where T : Delegate
    {
        lock (_lock)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _eventRegistrations.Add((typeof(T), action));

            return new Subscription(() =>
            {
                lock (_lock)
                {
                    _eventRegistrations.Remove((typeof(T), action));
                }
            });
        }
    }

    protected virtual void PublishInternal<T>(T? data)
    {
        Task.Run(() =>
        {
            lock (_lock)
            {
                foreach (var eventRegistration in _eventRegistrations)
                {
                    if (eventRegistration.Item2 is Action<T?> action)
                    {
                        action(data);
                    }

                    if (eventRegistration.Item2 is Func<T?> func)
                    {
                        func();
                    }
                }
            }
        }).Forget();
    }
}