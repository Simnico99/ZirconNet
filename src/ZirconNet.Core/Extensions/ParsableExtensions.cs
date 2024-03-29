﻿// <copyright file="ParsableExtensions.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace ZirconNet.Core.Extensions;

#if NET7_0_OR_GREATER
public static class ParsableExtensions
{
    public static T Parse<T>(this string input, IFormatProvider? formatProvider = null)
        where T : IParsable<T>
    {
        return T.Parse(input, formatProvider);
    }

    public static T Parse<T>(this ReadOnlySpan<char> input, IFormatProvider? formatProvider = null)
    where T : ISpanParsable<T>
    {
        return T.Parse(input, formatProvider);
    }

    public static bool TryParse<T>([NotNullWhen(true)] this string input, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T result)
    where T : IParsable<T>
    {
        return T.TryParse(input, formatProvider, out result);
    }

    public static bool TryParse<T>(this ReadOnlySpan<char> input, IFormatProvider? formatProvider, [MaybeNullWhen(false)] out T result)
    where T : ISpanParsable<T>
    {
        return T.TryParse(input, formatProvider, out result);
    }
}
#endif