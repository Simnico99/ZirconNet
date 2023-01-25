﻿using System.Runtime.CompilerServices;

namespace ZirconNet.Core.Async;

/// <summary>
/// Same as <see cref="Lazy{T}"/> but <see langword="async"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class LazyAsync<T> : Lazy<Task<T>>
{
    public LazyAsync(Func<T> valueFactory) :
        base(() => Task.Factory.StartNew(valueFactory))
    { }

    public LazyAsync(Func<Task<T>> taskFactory) :
        base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
    { }

    public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
}