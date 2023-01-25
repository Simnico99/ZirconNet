using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZirconNet.Core.Runtime;

public struct CustomIntEnumerator
{
    private int _current;
    private readonly int _end;

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


    public int Current => _current;

    public bool MoveNext()
    {
        _current++;
        return _current <= _end;
    }
}
