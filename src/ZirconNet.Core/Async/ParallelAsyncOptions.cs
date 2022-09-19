using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Async;
public readonly struct ParallelAsyncOptions
{
    public CancellationToken CancellationToken { get; init; } = default;
    public int MaxDegreeOfParallelism { get; init; } = -1;

    public ParallelAsyncOptions() { }
}
