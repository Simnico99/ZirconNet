// <copyright file="RangeExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZirconNet.Core.Runtime;

namespace ZirconNet.Core.Extensions;

public static class RangeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Inclusive(this int i)
    {
        return i + 1;
    }

#if NETCOREAPP3_1_OR_GREATER
    public static CustomIntEnumerator GetEnumerator(this Range range)
#else
    internal static CustomIntEnumerator GetEnumerator(this Range range)
#endif
    {
        return new CustomIntEnumerator(range);
    }

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
