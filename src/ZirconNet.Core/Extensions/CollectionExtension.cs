// <copyright file="CollectionExtension.cs" company="Zircon Technology">
// This software is distributed under the MIT license and its code is open-source and free for use, modification, and distribution.
// </copyright>

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
            _ = list.Remove(value);
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