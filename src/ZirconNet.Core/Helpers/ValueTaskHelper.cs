// <copyright file="ValueTaskHelper.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Helpers;

public static class ValueTaskHelper
{
    public static async ValueTask<T[]> WhenAll<T>(params ValueTask<T>[] tasks)
    {
        if (tasks == null)
        {
            throw new ArgumentNullException(nameof(tasks));
        }

        if (tasks.Length == 0)
        {
            return Array.Empty<T>();
        }

        List<Exception>? exceptions = null;

        var results = new T[tasks.Length];
        for (var i = 0; i < tasks.Length; i++)
        {
            try
            {
                results[i] = await tasks[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= new(tasks.Length);
                exceptions.Add(ex);
            }
        }

        return exceptions is null
            ? results
            : throw new AggregateException(exceptions);
    }

    public static ValueTask<T[]> WhenAll<T>(IReadOnlyList<ValueTask<T>> tasks)
    {
        return WhenAll(tasks);
    }

    public static ValueTask<T[]> WhenAll<T>(IEnumerable<ValueTask<T>> tasks)
    {
        return WhenAll(tasks);
    }

    public static async ValueTask WhenAll(params ValueTask[] tasks)
    {
        if (tasks == null)
        {
            throw new ArgumentNullException(nameof(tasks));
        }

        if (tasks.Length == 0)
        {
            return;
        }

        List<Exception>? exceptions = null;

        for (var i = 0; i < tasks.Length; i++)
        {
            try
            {
                await tasks[i].ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions ??= new(tasks.Length);
                exceptions.Add(ex);
            }
        }

        if (exceptions is null)
        {
            return;
        }

        throw new AggregateException(exceptions!);
    }

    public static ValueTask WhenAll(IReadOnlyList<ValueTask> tasks)
    {
        return WhenAll(tasks);
    }

    public static ValueTask WhenAll(IEnumerable<ValueTask> tasks)
    {
        return WhenAll(tasks);
    }
}
