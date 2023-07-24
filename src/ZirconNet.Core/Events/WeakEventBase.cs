// <copyright file="WeakEventBase.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Collections.Concurrent;
using ZirconNet.Core.Extensions;

namespace ZirconNet.Core.Events;

public abstract class WeakEventBase
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _eventRegistrations = new();

    protected virtual Subscription SubscribeInternal<T>(T action)
        where T : Delegate
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _eventRegistrations.AddOrUpdate(typeof(T), new List<Delegate> { action }, (key, value) =>
        {
            value.Add(action);
            return value;
        });

        return new Subscription(() => _eventRegistrations.AddOrUpdate(typeof(T), new List<Delegate>(), (key, value) =>
        {
            value.Remove(action);
            return value;
        }));
    }

    protected virtual void PublishInternal<T>(T? data)
    {
        Task.Run(() =>
        {
            if (_eventRegistrations.TryGetValue(typeof(T), out var actions))
            {
                foreach (var action in actions)
                {
                    ((Action<T?>)action)(data);
                }
            }
        }).Forget();
    }
}