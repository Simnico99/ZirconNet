namespace ZirconNet.Core.Extensions;
public static class ListExtension
{
    public static void AddThreadSafe<T>(this IList<T> list, T value)
    {
        lock (list)
        {
            list.Add(value);
        }
    }

    public static void RemoveThreadSafe<T>(this IList<T> list, T value)
    {
        lock (list)
        {
            list.Remove(value);
        }
    }

    public static int CountThreadSafe<T>(this IList<T> list)
    {
        lock (list)
        {
            return list.Count;
        }
    }
}

