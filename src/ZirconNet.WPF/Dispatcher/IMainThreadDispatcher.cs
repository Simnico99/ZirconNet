// <copyright file="IMainThreadDispatcher.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Windows.Threading;

namespace ZirconNet.WPF.Dispatcher;

/// <summary>
/// The main thread dispatcher singleton.
/// </summary>
public interface IMainThreadDispatcher
{
    /// <summary>
    /// Executes a specified delegate on the main UI thread with a specified priority.
    /// </summary>
    /// <param name="action">The delegate to run.</param>
    /// <param name="dispatcherPriority">The priority at which to execute the specified delegate. Default is DispatcherPriority.Send.</param>
    void Invoke(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send);

    /// <summary>
    /// Asynchronously executes a specified delegate on the main UI thread with a specified priority.
    /// </summary>
    /// <param name="action">The delegate to run.</param>
    /// <param name="dispatcherPriority">The priority at which to execute the specified delegate. Default is DispatcherPriority.Send.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask InvokeAsync(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send);

    /// <summary>
    /// Asynchronously executes a specified function delegate on the main UI thread with a specified priority and returns its result.
    /// </summary>
    /// <param name="func">The function delegate to run.</param>
    /// <param name="dispatcherPriority">The priority at which to execute the specified function. Default is DispatcherPriority.Send.</param>
    /// <typeparam name="T">The return type.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result is the return value of the function.</returns>
    ValueTask<T> InvokeAsync<T>(Func<T> func, DispatcherPriority dispatcherPriority = DispatcherPriority.Send);
}
