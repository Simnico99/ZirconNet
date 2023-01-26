using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Async;

/// <summary>
/// Same as <see cref="Lazy{T}"/> but <see langword="async"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class LazyAsync<T> : Lazy<Task<T>>
{
    public LazyAsync(Func<T> valueFactory) : base(() => Task.Run(valueFactory), LazyThreadSafetyMode.ExecutionAndPublication){ }

    public LazyAsync(Func<Task<T>> taskFactory) : base(taskFactory){ }

    public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
}