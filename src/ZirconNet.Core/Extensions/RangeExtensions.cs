using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZirconNet.Core.Runtime;

namespace ZirconNet.Core.Extensions;
public static class RangeExtensions
{
#if NETCOREAPP3_1_OR_GREATER
    public static CustomIntEnumerator GetEnumerator(this Range range)
#else
    internal static CustomIntEnumerator GetEnumerator(this Range range)
#endif
    {
        return new CustomIntEnumerator(range);
    }

#if NETCOREAPP3_1_OR_GREATER
    public static Range Inclusive(this Range range)
#else
    internal static Range Inclusive(this Range range) 
#endif
    {
        return new Range(range.Start, range.End.Value + 1);
    }
}
