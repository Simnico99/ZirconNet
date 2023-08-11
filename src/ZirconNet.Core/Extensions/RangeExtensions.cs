// <copyright file="RangeExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Runtime.CompilerServices;
using ZirconNet.Core.Runtime;

namespace ZirconNet.Core.Extensions;

/// <summary>
/// Provides extension methods for working with Range and integer types.
/// </summary>
public static class RangeExtensions
{
    /// <summary>
    /// Increases an integer by one.
    /// </summary>
    /// <param name="i">The integer to increase.</param>
    /// <returns>The integer value, incremented by one.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Inclusive(this int i)
    {
        return i + 1;
    }

    /// <summary>
    /// Gets an enumerator for the specified range.
    /// </summary>
    /// <param name="range">The range to enumerate.</param>
    /// <returns>A CustomIntEnumerator that can be used to iterate through the range.</returns>
#if NETCOREAPP3_1_OR_GREATER
    public static RangeEnumerator GetEnumerator(this Range range)
#else
    internal static RangeEnumerator GetEnumerator(this Range range)
#endif
    {
        return new RangeEnumerator(range);
    }

    /// <summary>
    /// Converts a range to an inclusive range.
    /// </summary>
    /// <param name="range">The range to convert.</param>
    /// <returns>A new Range with the same start point as the original and the end point increased by one.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NETCOREAPP3_1_OR_GREATER
    public static Range Inclusive(this Range range)
#else
    internal static Range Inclusive(this Range range)
#endif
    {
        return new Range(range.Start, range.End.Value + 1);
    }
}
