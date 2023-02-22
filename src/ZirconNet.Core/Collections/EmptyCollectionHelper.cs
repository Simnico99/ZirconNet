using System.Collections.ObjectModel;

namespace ZirconNet.Core.Collections;

//Not needed in Net8.0 as they added ReadOnlyCollections and ReadOnlyDictionnary Empty property.
#if NETFRAMEWORK || NET5_0 || NET6_0 || NET7_0
public static class EmptyCollectionHelper<T>
{
    public static ReadOnlyCollection<T> ReadOnlyCollection { get; } = new ReadOnlyCollection<T>(Array.Empty<T>());

    public static ReadOnlyObservableCollection<T> ReadOnlyObservableCollection { get; } = new ReadOnlyObservableCollection<T>(new ObservableCollection<T>());
}

public static class EmptyDictionnaryHelper<TKey, TValue> where TKey : notnull
{
    public static ReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary { get; } = new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
}
#endif