namespace Psy.Graphics
{
    public interface IDataStream<T>
    {
        void WriteRange(T[] items);
        void Write(T item);
    }
}