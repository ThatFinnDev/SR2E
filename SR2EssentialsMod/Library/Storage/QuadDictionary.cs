#pragma warning disable
namespace CottonLibrary.Storage
{
    public class QuadDictionary<TKey, TValue1, TValue2, TValue3> : Dictionary<TKey, (TValue1, TValue2, TValue3)>
    {
        public QuadDictionary(int capacity = 0) : base(capacity)
        {

        }

        public void AddItems(TKey key, TValue1 value1, TValue2 value2, TValue3 value3)
        {
            Add(key, (value1, value2, value3));
        }
    }
}
