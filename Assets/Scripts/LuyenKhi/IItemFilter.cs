namespace LuyenKhi
{
    public interface IItemFilter<T>
    {
        bool Filter(T item);
    }
}