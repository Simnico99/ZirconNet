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
        var buffer = new char[source.Length];

        for (var i = 0; i < source.Length; i++)
        {
            buffer[i] = source[i] == oldChar ? newChar : source[i];
        }

        return buffer;
    }

    public static ReadOnlySpan<char> Replace(this ReadOnlySpan<char> source, ReadOnlySpan<char> oldSpan, ReadOnlySpan<char> newSpan)
    {
        var countOfOldSpan = 0;
        var index = source.IndexOf(oldSpan);
        while (index != -1)
        {
            countOfOldSpan++;
            index = source[(index + oldSpan.Length)..].IndexOf(oldSpan);
        }

        var resultLength = source.Length + ((newSpan.Length - oldSpan.Length) * countOfOldSpan);
        var resultBuffer = new char[resultLength];

        var writeIndex = 0;
        var readIndex = 0;

        index = source.IndexOf(oldSpan);
        while (index != -1)
        {
            source[readIndex..index].CopyTo(resultBuffer.AsSpan()[writeIndex..]);
            writeIndex += index - readIndex;
            newSpan.CopyTo(resultBuffer.AsSpan()[writeIndex..]);
            writeIndex += newSpan.Length;

            readIndex = index + oldSpan.Length;
            index = source[readIndex..].IndexOf(oldSpan);
        }

        source[readIndex..].CopyTo(resultBuffer.AsSpan()[writeIndex..]);

        return resultBuffer;
    }

    private static T ParseSlice<T>(ReadOnlySpan<char> slice, TryParseHandler<T> tryParseHandler)
    {
        tryParseHandler(slice, out var result);
        return result;
    }
}
