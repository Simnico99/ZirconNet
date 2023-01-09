using System.Collections.Concurrent;

namespace ZirconNet.Core.Async;
public static class ParallelAsync
{
    public static Task ForEach<T>(this IEnumerable<T> source, in ParallelAsyncOptions parallelOptions, Func<T, Task> body)
    {
        var maxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism;

        if (parallelOptions.MaxDegreeOfParallelism <= 0)
        {
            maxDegreeOfParallelism = Environment.ProcessorCount;
        }

        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                { await body(partition.Current); }
            }
        }

        return parallelOptions.CancellationToken.IsCancellationRequested
            ? throw new TaskCanceledException()
            : Task.WhenAll(Partitioner
                .Create(source)
                .GetPartitions(parallelOptions.MaxDegreeOfParallelism)
                .AsParallel()
                .Select(AwaitPartition));
    }

    public static Task ForEach<T>(this IEnumerable<T> source, int maxDegreeOfParallelism = -1, Func<T, Task>? body = null)
    {
        if (maxDegreeOfParallelism <= 0)
        {
            maxDegreeOfParallelism = Environment.ProcessorCount;
        }

        if (body is null)
        {
            throw new ArgumentNullException(nameof(body));
        }

        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                { await body(partition.Current); }
            }
        }

        return Task.WhenAll(Partitioner
                .Create(source)
                .GetPartitions(maxDegreeOfParallelism)
                .AsParallel()
                .Select(AwaitPartition));
    }
}
