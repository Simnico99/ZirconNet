// <copyright file="Subscription.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Events;

public readonly struct Subscription : IDisposable
{
    private readonly Action _removeMethod;

    public Subscription(Action removeMethod)
    {
        _removeMethod = removeMethod;
    }

    public void Dispose()
    {
        if (_removeMethod is not null)
        {
            _removeMethod();
        }
    }
}
