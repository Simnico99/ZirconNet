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

    private static T ParseSlice<T>(ReadOnlySpan<char> slice, TryParseHandler<T> tryParseHandler)
    {
        tryParseHandler(slice, out var result);
        return result;
    }
}
