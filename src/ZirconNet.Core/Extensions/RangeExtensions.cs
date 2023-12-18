// <copyright file="RangeExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Runtime;

namespace ZirconNet.Core.Extensions;

/// <summary>
/// Provides extension methods for working with Range and integer types.
/// </summary>
public static class RangeExtensions
{
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
}
