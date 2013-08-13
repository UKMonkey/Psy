using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Psy.Graphics.OpenGL
{
    public class DataStream<T> : IDataStream<T> where T : struct
    {
        private readonly VertexRenderer<T> _vertexRenderer;
        private int _index;

        public DataStream(VertexRenderer<T> vertexRenderer)
        {
            _vertexRenderer = vertexRenderer;
            _index = 0;
        }

        public void WriteRange(T[] items)
        {
            if (items.Length == _vertexRenderer.LocalCopy.Length)
            {
                Array.Copy(items, _vertexRenderer.LocalCopy, items.Length);
            }
            else
            {
                foreach (var item in items)
                {
                    _vertexRenderer.LocalCopy[_index] = item;
                    _index++;
                }
            }

        }

        public void Write(T item)
        {
            _vertexRenderer.LocalCopy[_index] = item;
            _index++;
        }

        public void Flush()
        {
            Debug.Assert(_index <= _vertexRenderer.LocalCopy.Length);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexRenderer.BufferId); GLH.AssertGLError();
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(Marshal.SizeOf(typeof(T)) * _vertexRenderer.LocalCopy.Length),
                _vertexRenderer.LocalCopy,
                BufferUsageHint.StreamDraw); GLH.AssertGLError();
        }
    }
}