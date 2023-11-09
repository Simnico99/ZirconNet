// <copyright file="TupleTaskExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;
using ZirconNet.Core.Helpers;

namespace ZirconNet.Core.Extensions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1414:Tuple types in signatures should have element names", Justification = "Wont be used.")]
public static class TupleTaskExtensions
{
    public static TaskAwaiter<(T1, T2)> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) taskTuple)
    {
        async Task<(T1, T2)> CombineTasks()
        {
            var (task1, task2) = taskTuple;
            await TaskHelper.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    public static TaskAwaiter<(T1, T2, T3)> GetAwaiter<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) taskTuple)
    {
        async Task<(T1, T2, T3)> CombineTasks()
        {
            var (task1, task2, task3) = taskTuple;
            await TaskHelper.WhenAll(task1, task2, task3);
            return (task1.Result, task2.Result, task3.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    public static TaskAwaiter<(T1, T2, T3, T4)> GetAwaiter<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) taskTuple)
    {
        async Task<(T1, T2, T3, T4)> CombineTasks()
        {
            var (task1, task2, task3, task4) = taskTuple;
            await TaskHelper.WhenAll(task1, task2, task3, task4);
            return (task1.Result, task2.Result, task3.Result, task4.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    public static TaskAwaiter<(T1, T2, T3, T4, T5)> GetAwaiter<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) taskTuple)
    {
        async Task<(T1, T2, T3, T4, T5)> CombineTasks()
        {
            var (task1, task2, task3, task4, task5) = taskTuple;
            await TaskHelper.WhenAll(task1, task2, task3, task4, task5);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    public static TaskAwaiter<(T1, T2, T3, T4, T5, T6)> GetAwaiter<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) taskTuple)
    {
        async Task<(T1, T2, T3, T4, T5, T6)> CombineTasks()
        {
            var (task1, task2, task3, task4, task5, task6) = taskTuple;
            await TaskHelper.WhenAll(task1, task2, task3, task4, task5, task6);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    public static TaskAwaiter<(T1, T2, T3, T4, T5, T6, T7)> GetAwaiter<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) taskTuple)
    {
        async Task<(T1, T2, T3, T4, T5, T6, T7)> CombineTasks()
        {
            var (task1, task2, task3, task4, task5, task6, task7) = taskTuple;
            await TaskHelper.WhenAll(task1, task2, task3, task4, task5, task6, task7);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result, task7.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    public static TaskAwaiter<(T1, T2, T3, T4, T5, T6, T7, T8)> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) taskTuple)
    {
        async Task<(T1, T2, T3, T4, T5, T6, T7, T8)> CombineTasks()
        {
            var (task1, task2, task3, task4, task5, task6, task7, task8) = taskTuple;
            await TaskHelper.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result, task7.Result, task8.Result);
        }

        return CombineTasks().GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this (Task, Task) taskTuple)
    {
        return TaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2).GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this (Task, Task, Task) taskTuple)
    {
        return TaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3).GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task) taskTuple)
    {
        return TaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4).GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task) taskTuple)
    {
        return TaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5).GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task) taskTuple)
    {
        return TaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6).GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task) taskTuple)
    {
        return TaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6, taskTuple.Item7).GetAwaiter();
    }

    public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task) taskTuple)
    {
        return TaskHelper.WhenAll(taskTuple.Item1, taskTuple.Item2, taskTuple.Item3, taskTuple.Item4, taskTuple.Item5, taskTuple.Item6, taskTuple.Item7, taskTuple.Item8).GetAwaiter();
    }
}
