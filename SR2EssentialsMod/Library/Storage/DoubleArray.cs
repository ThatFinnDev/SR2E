
namespace SR2E.Library.Storage
{
    public class DoubleArray<T0, T1>
    {
        public (T0, T1)[] items;

        public DoubleArray(long size = 0)
        {
            items = new (T0, T1)[size];
        }
    }
}
