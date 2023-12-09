namespace SR2E.Library.Storage
{
#pragma warning disable

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
