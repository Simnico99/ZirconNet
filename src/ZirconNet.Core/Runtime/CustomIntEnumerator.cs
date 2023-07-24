// <copyright file="CustomIntEnumerator.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Runtime;

public struct CustomIntEnumerator
{
    private readonly int _end;
    private int _current;

#if NETCOREAPP3_1_OR_GREATER
    public CustomIntEnumerator(Range range)
#else
    internal CustomIntEnumerator(Range range)
#endif
    {
        if (range.End.IsFromEnd)
        {
            throw new NotSupportedException();
        }

        _current = range.Start.Value - 1;
        _end = range.End.Value;
    }

    public readonly int Current => _current;

    public bool MoveNext()
    {
        _current++;
        return _current <= _end;
    }
}