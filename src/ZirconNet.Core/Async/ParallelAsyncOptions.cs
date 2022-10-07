namespace ZirconNet.Core.Async;
public readonly struct ParallelAsyncOptions
{
    private static ParallelAsyncOptions? _internalDefaultStuct;
    public static ParallelAsyncOptions Default => _internalDefaultStuct ??= new();
#if NET5_0_OR_GREATER
    public CancellationToken CancellationToken { get; init; } = default;
    public int MaxDegreeOfParallelism { get; init; } = -1;
    public ParallelAsyncOptions() { }
#else
    public CancellationToken CancellationToken { get; }
    public int MaxDegreeOfParallelism { get; }
#endif
    public ParallelAsyncOptions(int maxDegreeOfParallelism = -1, CancellationToken cancellationToken = default)
    {
        CancellationToken = cancellationToken;
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
    }
}
