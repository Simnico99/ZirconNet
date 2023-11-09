// <copyright file="TupleValueTaskExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;
using ZirconNet.Core.Helpers;

namespace ZirconNet.Core.Extensions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "Wont be used.")]
public static class TupleValueTaskExtensions
{
    public static ValueTaskAwaiter<T[]> GetAwaiter<T>(this (ValueTask<T>, ValueTask<T>) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2).GetAwaiter();
    }

    public static ValueTaskAwaiter<T[]> GetAwaiter<T>(this (ValueTask<T>, ValueTask<T>, ValueTask<T>) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3).GetAwaiter();
    }

    public static ValueTaskAwaiter<T[]> GetAwaiter<T>(this (ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4).GetAwaiter();
    }

    public static ValueTaskAwaiter<T[]> GetAwaiter<T>(this (ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5).GetAwaiter();
    }

    public static ValueTaskAwaiter<T[]> GetAwaiter<T>(this (ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6).GetAwaiter();
    }

    public static ValueTaskAwaiter<T[]> GetAwaiter<T>(this (ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6, taskTuple.Item7).GetAwaiter();
    }

    public static ValueTaskAwaiter<T[]> GetAwaiter<T>(this (ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>, ValueTask<T>) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6, taskTuple.Item7, taskTuple.Item8).GetAwaiter();
    }

    public static ValueTaskAwaiter GetAwaiter(this (ValueTask, ValueTask) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2).GetAwaiter();
    }

    public static ValueTaskAwaiter GetAwaiter(this (ValueTask, ValueTask, ValueTask) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3).GetAwaiter();
    }

    public static ValueTaskAwaiter GetAwaiter(this (ValueTask, ValueTask, ValueTask, ValueTask) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4).GetAwaiter();
    }

    public static ValueTaskAwaiter GetAwaiter(this (ValueTask, ValueTask, ValueTask, ValueTask, ValueTask) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5).GetAwaiter();
    }

    public static ValueTaskAwaiter GetAwaiter(this (ValueTask, ValueTask, ValueTask, ValueTask, ValueTask, ValueTask) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6).GetAwaiter();
    }

    public static ValueTaskAwaiter GetAwaiter(this (ValueTask, ValueTask, ValueTask, ValueTask, ValueTask, ValueTask, ValueTask) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6, taskTuple.Item7).GetAwaiter();
    }

    public static ValueTaskAwaiter GetAwaiter(this (ValueTask, ValueTask, ValueTask, ValueTask, ValueTask, ValueTask, ValueTask, ValueTask) taskTuple)
    {
        return ValueTaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6, taskTuple.Item7, taskTuple.Item8).GetAwaiter();
    }
}
