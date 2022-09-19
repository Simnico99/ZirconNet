using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;
public static class ParallelAsync
{
    public static Task ForEach<T>(this IEnumerable<T> source, ParallelOptions parallelOptions, Func<T, Task> body)
    {
        if (parallelOptions.MaxDegreeOfParallelism <= 0)
        {
            parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
        }

        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                { await body(partition.Current); }
            }
        }

        if (parallelOptions.CancellationToken.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }

        return Task.WhenAll(Partitioner
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
