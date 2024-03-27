// <copyright file="TimeSpanExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Extensions;

public static class TimeSpanExtensions
{
    public static string ToMostRelevantUnit(this TimeSpan timeSpan)
    {
#if NET7_0_OR_GREATER
        var nanoseconds = timeSpan.TotalNanoseconds;
#else
        var nanoseconds = timeSpan.TotalNanoseconds();
#endif
        return nanoseconds switch
        {
            >= 3_600_000_000_000 => $"{nanoseconds / 3_600_000_000_000:0.##}h",
            >= 60_000_000_000 => $"{nanoseconds / 60_000_000_000:0.##}m",
            >= 1_000_000_000 => $"{nanoseconds / 1_000_000_000:0.##}s",
            >= 1_000_000 => $"{nanoseconds / 1_000_000:0.##}ms",
            >= 1_000 => $"{nanoseconds / 1_000:0.##}µs",
            _ => $"{nanoseconds}ns",
        };
    }

#if !NET7_0_OR_GREATER
    public static double TotalNanoseconds(this TimeSpan timeSpan)
    {
        return timeSpan.Ticks * 100.0;
    }
#endif
}
