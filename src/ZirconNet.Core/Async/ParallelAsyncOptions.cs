// <copyright file="ParallelAsyncOptions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Async;

public readonly struct ParallelAsyncOptions
{
    private static ParallelAsyncOptions? _internalDefaultStuct;

#if NET5_0_OR_GREATER
    public ParallelAsyncOptions()
    {
    }
#endif

    public ParallelAsyncOptions(int maxDegreeOfParallelism = -1, CancellationToken cancellationToken = default)
    {
        CancellationToken = cancellationToken;
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    public static ParallelAsyncOptions Default => _internalDefaultStuct ??= default;

#if NET5_0_OR_GREATER
    public CancellationToken CancellationToken { get; init; } = default;

    public int MaxDegreeOfParallelism { get; init; } = -1;

#else
    public CancellationToken CancellationToken { get; }

    public int MaxDegreeOfParallelism { get; }

#endif
}
