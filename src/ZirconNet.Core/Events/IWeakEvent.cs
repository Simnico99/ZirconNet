// <copyright file="IWeakEvent.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Events;

/// <summary>
/// A weak event that can be subscribed to and published.
/// </summary>
public interface IWeakEvent
{
    /// <summary>
    /// Publishes the event.
    /// </summary>
    void Publish();

    /// <summary>
    /// Subscribes to the event.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A subscription item. (Can be disposed).</returns>
    Subscription Subscribe(Action action);

    /// <summary>
    /// Subscribes to the event.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <returns>A subscription item. (Can be disposed).</returns>
    Subscription Subscribe<T>(Func<T> action);
}