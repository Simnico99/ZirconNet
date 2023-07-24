// <copyright file="LazyAsync.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Async;

/// <summary>
/// Same as <see cref="Lazy{T}"/> but <see langword="async"/>.
/// </summary>
/// <typeparam name="T">Return type.</typeparam>
public sealed class LazyAsync<T> : Lazy<Task<T>>
{
    public LazyAsync(Func<T> valueFactory)
        : base(() => Task.Run(valueFactory), LazyThreadSafetyMode.ExecutionAndPublication)
    {
    }

    public LazyAsync(Func<Task<T>> taskFactory)
        : base(taskFactory)
    {
    }

    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}