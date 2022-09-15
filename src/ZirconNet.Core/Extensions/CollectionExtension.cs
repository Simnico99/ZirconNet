namespace ZirconNet.Core.Extensions;
public static class CollectionExtension
{
    public static void AddThreadSafe<T>(this ICollection<T> list, T value)
    {
        lock (list)
        {
            list.Add(value);
        }
    }

    public static void RemoveThreadSafe<T>(this ICollection<T> list, T value)
    {
        lock (list)
        {
            list.Remove(value);
        }
    }

    public static int CountThreadSafe<T>(this ICollection<T> list)
    {
        lock (list)
        {
            return list.Count;
        }
    }
}