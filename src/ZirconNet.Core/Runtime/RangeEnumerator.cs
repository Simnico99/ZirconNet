﻿// <copyright file="RangeEnumerator.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Runtime;

/// <summary>
/// Defines an enumerator for a range of integers.
/// </summary>
public struct RangeEnumerator
{
    private readonly int _end;
    private int _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeEnumerator"/> struct.
    /// </summary>
    /// <param name="range">The range to enumerate.</param>
    /// <exception cref="NotSupportedException">Thrown when the end of the range is from the end.</exception>
#if NETCOREAPP3_1_OR_GREATER
    public RangeEnumerator(Range range)
#else
    internal RangeEnumerator(Range range)
#endif
    {
        if (range.End.IsFromEnd)
        {
            throw new NotSupportedException();
        }

        _current = range.Start.Value - 1;
        _end = range.End.Value;
    }

    /// <summary>
    /// Gets the current integer in the enumerated range.
    /// </summary>
    public readonly int Current => _current;

    /// <summary>
    /// Advances the enumerator to the next integer in the range.
    /// </summary>
    /// <returns>True if the enumerator was successfully advanced to the next integer; false if the enumerator has passed the end of the range.</returns>
    public bool MoveNext()
    {
        _current++;
        return _current <= _end;
    }
}
