#if NETCOREAPP3_1_OR_GREATER
namespace ZirconNet.Core.Extensions;
public static class SpanCharExtension
{
    public static Span<char> Remove(this Span<char> chars, char charToRemove)
    {
        Span<char> span = new char[chars.Length];

        var charPos = 0;
        foreach (var character in chars)
        {
            if (character == charToRemove)
            {
                continue;
            }

            span[++charPos] = character;
        }

        return span.TrimEnd();
    }

    public static Span<char> Remove(this Span<char> chars, char[] charsToRemove)
    {
        Span<char> span = new char[chars.Length];

        var charPos = 0;
        foreach (var character in chars)
        {
            if (charsToRemove.Contains(character))
            {
                continue;
            }

            span[++charPos] = character;
        }

        return span.TrimEnd();
    }
}
#endif