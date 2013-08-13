using SlimDX;

namespace Psy.Graphics.DirectX
{
    public class WrappedDataStream<T> : IDataStream<T> where T : struct
    {
        private readonly DataStream _dataStream;

        public WrappedDataStream(DataStream dataStream)
        {
            _dataStream = dataStream;
        }

        public void WriteRange(T[] items)
        {
            _dataStream.WriteRange(items);
        }

        public void Write(T item)
        {
            _dataStream.Write(item);
        }
    }
}