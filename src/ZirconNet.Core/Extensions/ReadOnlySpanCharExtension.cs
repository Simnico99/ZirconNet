// <copyright file="ReadOnlySpanCharExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

using ZirconNet.Core.Delegates;

namespace ZirconNet.Core.Extensions;

public static class ReadOnlySpanCharExtension
{
    public static T[] SplitToArray<T>(this ReadOnlySpan<char> span, char comparison, TryParseHandler<T> tryParseHandler, int startIndex = 0, int sliceLength = 0, int size = 0)
    {
        if (size < 1)
        {
            size = span.Length;
        }

        if (sliceLength <= 0)
        {
            sliceLength = span.Length;
        }

        var array = new T[size];
        var arrayIndex = 0;
        var currentIndex = startIndex;
        var length = 0;
        var endIndex = currentIndex + sliceLength - 1;

        for (var i = currentIndex; i <= endIndex && arrayIndex < array.Length; i++)
        {
            length++;

            if (span[i] == comparison)
            {
                array[arrayIndex++] = ParseSlice(span.Slice(currentIndex, length - 1), tryParseHandler);
                currentIndex += length;
                length = 0;
            }
            else if (i == endIndex)
            {
                array[arrayIndex++] = ParseSlice(span.Slice(currentIndex, length), tryParseHandler);
            }
        }

        return array;
    }

    public static ReadOnlySpan<char> Replace(this ReadOnlySpan<char> source, char oldChar, char newChar)
    {
        char[]? buffer = null;

        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] == oldChar)
            {
                buffer ??= source.ToArray();
                buffer[i] = newChar;
            }
        }

        return buffer == null ? source : new ReadOnlySpan<char>(buffer);
    }

    private static T ParseSlice<T>(ReadOnlySpan<char> slice, TryParseHandler<T> tryParseHandler)
    {
        tryParseHandler(slice, out var result);
        return result;
    }
}
