
namespace SR2E.Library.Storage
{
    public class DoubleList<T0, T1> : List<(T0, T1)>
    {
        public DoubleList(int capacity = 0) : base(capacity)
        {

        }

        public void AddItems(T0 item1, T1 item2)
        {
            Add((item1, item2));
        }
    }
}
