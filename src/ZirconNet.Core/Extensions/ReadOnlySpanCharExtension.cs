using ZirconNet.Core.Delegates;

namespace ZirconNet.Core.Extensions;

public static class ReadOnlySpanCharExtension
{
    public static T[] SplitToArray<T>(this ReadOnlySpan<char> span, char comparison, TryParseHandler<T> tryParseHandler, int startIndex = 0, int sliceLenght = 0, int size = 0)
    {
        T[]? array;
        if (size < 1)
        {
            array = new T[span.Length];
        }
        else
        {
            array = new T[size];
        }

        if (sliceLenght <= 0)
        {
            sliceLenght = span.Length;
        }

        sliceLenght--;

        var length = 0;
        var arrayIndex = 0;
        var currentIndex = startIndex;
        foreach (var i in currentIndex..(currentIndex + sliceLenght))
        {
            length++;

            if (arrayIndex == array.Length)
            {
                break;
            }

            if (span[i] == comparison)
            {
                var slice = span.Slice(currentIndex, length - 1);
                tryParseHandler(slice, out var result);
                array[arrayIndex] = result;
                currentIndex += length;
                arrayIndex++;
                length = 0;
                continue;
            }

            if (i == (startIndex + sliceLenght))
            {
                var slice = span.Slice(currentIndex, length);
                tryParseHandler(slice, out var result);
                array[arrayIndex] = result;
            }
        }

        return array;
    }
}
