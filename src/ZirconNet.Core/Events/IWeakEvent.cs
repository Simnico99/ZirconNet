// <copyright file="IWeakEvent.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Events;

public interface IWeakEvent
{
    void Publish();

    Subscription Subscribe(Action action);

    Subscription Subscribe<T>(Func<T> action);
}