namespace SR2E.Storage
{


    /// <summary>
    /// A dictionary with 2 values per key.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue1">The first values' type</typeparam>
    /// <typeparam name="TValue2">The second values` type</typeparam>
    public class TripleDictionary<TKey, TValue1, TValue2> : Dictionary<TKey, (TValue1, TValue2)>
    {
        public TripleDictionary(int capacity = 0) : base(capacity)
        {
    
        }

        public void AddItems(TKey key, TValue1 value1, TValue2 value2)
        {
            Add(key, (value1, value2));
        }
    }
}
