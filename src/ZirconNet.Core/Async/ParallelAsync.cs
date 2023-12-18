// <copyright file="ParallelAsync.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Collections.Concurrent;

namespace ZirconNet.Core.Async;

public static class ParallelAsync
{
    public static Task ForEach<T>(this IEnumerable<T> source, in ParallelAsyncOptions parallelOptions, Func<T, Task> body)
    {
        return ForEachInternal(source, parallelOptions.MaxDegreeOfParallelism, body, parallelOptions.CancellationToken);
    }

    public static Task ForEach<T>(this IEnumerable<T> source, int maxDegreeOfParallelism = -1, Func<T, Task>? body = null)
    {
        return ForEachInternal(source, maxDegreeOfParallelism, body ?? throw new ArgumentNullException(nameof(body)));
    }

    public static Task ForEach<T>(this IEnumerable<T> source, Func<T, Task>? body = null)
    {
        return ForEachInternal(source, -1, body ?? throw new ArgumentNullException(nameof(body)));
    }

    public static Task ForEach<T>(this T[] source, Func<T, Task>? body = null)
    {
        return ForEachInternal(source, -1, body ?? throw new ArgumentNullException(nameof(body)));
    }

    private static Task ForEachInternal<T>(IEnumerable<T> source, int maxDegreeOfParallelism, Func<T, Task> body, CancellationToken cancellationToken = default)
    {
        if (maxDegreeOfParallelism <= 0)
        {
            maxDegreeOfParallelism = Environment.ProcessorCount;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await body(partition.Current);
                }
            }
        }

        return Task.WhenAll(Partitioner
            .Create(source)
            .GetPartitions(maxDegreeOfParallelism)
            .AsParallel()
            .Select(AwaitPartition));
    }
}
