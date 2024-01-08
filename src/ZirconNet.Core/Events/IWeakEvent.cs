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
    void Subscribe(Action action);

    /// <summary>
    /// Subscribes to the event.
    /// </summary>
    /// <typeparam name="T">The func type.</typeparam>
    /// <param name="action">The action to execute.</param>
    void Subscribe<T>(Func<T> action);

    /// <summary>
    /// Unsubscribe to the event.
    /// </summary>
    /// <param name="action">The action that was to execute.</param>
    /// <returns>If the item has been found and removed.</returns>
    bool Unsubscribe(Action action);

    /// <summary>
    /// Unsubscribe to the event.
    /// </summary>
    /// <typeparam name="T">The func type.</typeparam>
    /// <param name="action">The action that was to execute.</param>
    /// <returns>If the item has been found and removed.</returns>
    bool Unsubscribe<T>(Func<T> action);
}