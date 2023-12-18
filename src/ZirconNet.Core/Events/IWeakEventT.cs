// <copyright file="IWeakEventT.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Events;

/// <summary>
/// A weak event that can be subscribed to and published.
/// </summary>
/// <typeparam name="T">The return type.</typeparam>
public interface IWeakEvent<T>
{
    /// <summary>
    /// Publishes the event.
    /// </summary>
    /// <param name="data">The data to publish.</param>
    void Publish(T data);

    /// <summary>
    /// Subscribes to the event.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void Subscribe(Action<T> action);

    /// <summary>
    /// Subscribes to the event.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    void Subscribe(Func<T> action);

    /// <summary>
    /// Unsubscribe to the event.
    /// </summary>
    /// <param name="action">The action that was to execute.</param>
    void Unsubscribe(Action<T> action);

    /// <summary>
    /// Unsubscribe to the event.
    /// </summary>
    /// <param name="action">The action that was to execute.</param>
    void Unsubscribe(Func<T> action);
}