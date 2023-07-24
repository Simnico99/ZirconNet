// <copyright file="SpanCharExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

namespace ZirconNet.Core.Extensions;

public static class SpanCharExtension
{
    public static Span<char> Remove(this Span<char> chars, char charToRemove)
    {
        var charPos = 0;
        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] != charToRemove)
            {
                chars[charPos++] = chars[i];
            }
        }

        return chars[..charPos];
    }

    public static Span<char> Remove(this Span<char> chars, char[] charsToRemove)
    {
        var charPos = 0;
        for (var i = 0; i < chars.Length; i++)
        {
            var shouldRemove = false;
            for (var j = 0; j < charsToRemove.Length; j++)
            {
                if (chars[i] == charsToRemove[j])
                {
                    shouldRemove = true;
                    break;
                }
            }

            if (!shouldRemove)
            {
                chars[charPos++] = chars[i];
            }
        }

        return chars[..charPos];
    }
}