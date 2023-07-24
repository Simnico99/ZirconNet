// <copyright file="IMainThreadDispatcher.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;

public interface IMainThreadDispatcher
{
    void Invoke(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send);

    ValueTask InvokeAsync(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send);

    ValueTask<T> InvokeAsync<T>(Func<T> func, DispatcherPriority dispatcherPriority = DispatcherPriority.Send);
}