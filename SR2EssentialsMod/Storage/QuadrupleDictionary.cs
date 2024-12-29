namespace SR2E.Storage;

/// <summary>
/// A dictionary with 3 values per key.
/// </summary>
/// <typeparam name="TKey">The key type</typeparam>
/// <typeparam name="TValue1">The first values' type</typeparam>
/// <typeparam name="TValue2">The second values` type</typeparam>
/// <typeparam name="TValue3">The third values` type</typeparam>
public class QuadrupleDictionary<TKey, TValue1, TValue2, TValue3> : Dictionary<TKey, (TValue1, TValue2, TValue3)>
{
    public QuadrupleDictionary(int capacity = 0) : base(capacity)
    {
    
    }

    public void AddItems(TKey key, TValue1 value1, TValue2 value2, TValue3 value3)
    {
        Add(key, (value1, value2, value3));
    }
}
