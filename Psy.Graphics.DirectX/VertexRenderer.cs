using System;
using System.Runtime.InteropServices;
using Psy.Core;
using Psy.Graphics.DirectX.Textures;
using SlimDX;
using SlimDX.Direct3D9;

namespace Psy.Graphics.DirectX
{
    public class VertexRenderer<T> : IVertexRenderer<T> where T : struct
    {
        private readonly Device _device;
        private readonly VertexBuffer _vertexBuffer;
        private readonly VertexDeclaration _vertexDeclaration;
        private readonly IndexBuffer _indexBuffer;
        private bool _vertexBufferLocked;
        private bool _indexBufferLocked;
        private readonly int _typeSize = Marshal.SizeOf(typeof(T));
        private readonly bool _indexed;
        private readonly int _vertexCount;

        public void Dispose()
        {
            if (_vertexBuffer != null)
                _vertexBuffer.Dispose();
        }

        public static IVertexRenderer<T> Create(VertexDeclarationStorage vertexDeclarationStorage,
            int vertexCount, Device device, Usage usage = Usage.WriteOnly, Pool pool = Pool.Managed)
        {
            return new VertexRenderer<T>(vertexDeclarationStorage, vertexCount, device, usage, pool);
        }

        public static VertexRenderer<T> CreateIndexed(
            VertexDeclarationStorage vertexDeclarationStorage,
            int vertexCount, int indexCount, Device device,
            Usage usage = Usage.WriteOnly, Pool pool = Pool.Managed)
        {
            return new VertexRenderer<T>(vertexDeclarationStorage, vertexCount, indexCount, device, usage, pool);    
        }

        private VertexRenderer(VertexDeclarationStorage vertexDeclarationStorage, int vertexCount, 
            int indexCount, Device device, Usage usage, Pool pool)
        {
            _vertexCount = vertexCount;
            _indexed = true;
            _device = device;
            _vertexBuffer = new VertexBuffer(_device, _vertexCount * _typeSize, usage, VertexFormat.None, pool);
            _indexBuffer = new IndexBuffer(_device, indexCount * sizeof(short), Usage.WriteOnly, pool, true);
            _vertexDeclaration = vertexDeclarationStorage.GetFor<T>(_device);
            _vertexBufferLocked = false;
            _indexBufferLocked = false;
        }

        private VertexRenderer(VertexDeclarationStorage vertexDeclarationStorage, int vertexCount, 
            Device device, Usage usage, Pool pool)
        {
            _vertexCount = vertexCount;
            _indexed = false;
            _device = device;
            _vertexBuffer = new VertexBuffer(
                _device,
                _vertexCount * _typeSize, 
                usage, 
                VertexFormat.None, 
                pool);
            _vertexDeclaration = vertexDeclarationStorage.GetFor<T>(device);
            _vertexBufferLocked = false;
        }

        public IDataStream<T> LockVertexBuffer()
        {
            if (_vertexBufferLocked)
            {
                throw new InvalidOperationException("Vertex buffer is already locked.");
            }
            _vertexBufferLocked = true;

            var dataStream = _vertexBuffer.Lock(0, 0, LockFlags.None);
            return new WrappedDataStream<T>(dataStream);
        }

        public void UnlockVertexBuffer()
        {
            if (!_vertexBufferLocked)
                throw new InvalidOperationException("Vertex buffer is not locked - cannot unlock.");
            _vertexBuffer.Unlock();
            _vertexBufferLocked = false;
        }

        public IDataStream<short> LockIndexBuffer()
        {
            if (!_indexed)
            {
                throw new InvalidOperationException("Index buffer does not exist.");
            }

            if (_indexBufferLocked)
            {
                throw new InvalidOperationException("Index buffer is already locked.");
            }
            _indexBufferLocked = true;
            
            var dataStream = _indexBuffer.Lock(0, 0, LockFlags.None);
            return new WrappedDataStream<short>(dataStream);
        }

        public void UnlockIndexBuffer()
        {
            if (!_indexed)
            {
                throw new InvalidOperationException("Index buffer does not exist.");
            }

            if (!_indexBufferLocked)
            {
                throw new InvalidOperationException("Index buffer is not locked - cannot unlock.");
            }
            _indexBuffer.Unlock();
            _indexBufferLocked = false;
        }

        public void Render(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            if (_indexed)
            {
                throw new NotImplementedException("Use RenderIndexed");
            }

            RenderNonIndexed(primitiveType, startIndex, primitiveCount);
        }

        public void RenderIndexed(PrimitiveType primitiveType, int primitiveCount)
        {
            _device.SetStreamSource(0, _vertexBuffer, 0, _typeSize);
            _device.VertexDeclaration = _vertexDeclaration;
            _device.Indices = _indexBuffer;
            _device.DrawIndexedPrimitives(PrimitiveTypeMapper.MapFrom(primitiveType), 0, 0, _vertexCount, 0, primitiveCount);
        }

        public void RenderNonIndexed(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            AssertBufferNotLocked();

            _device.SetStreamSource(0, _vertexBuffer, 0, _typeSize);
            _device.VertexDeclaration = _vertexDeclaration;
            _device.SetTexture(0, null);
            _device.DrawPrimitives(PrimitiveTypeMapper.MapFrom(primitiveType), startIndex, primitiveCount);
        }

        public void RenderNoTexture(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            AssertBufferNotLocked();

            _device.SetStreamSource(0, _vertexBuffer, 0, _typeSize);
            _device.VertexDeclaration = _vertexDeclaration;
            _device.DrawPrimitives(PrimitiveTypeMapper.MapFrom(primitiveType), startIndex, primitiveCount);
        }

        private void AssertBufferNotLocked()
        {
            if (_vertexBufferLocked)
                throw new InvalidOperationException("Buffer is locked, call will fail!");
        }

        public void RenderForShader(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            AssertBufferNotLocked();

            _device.SetStreamSource(0, _vertexBuffer, 0, _typeSize);
            _device.VertexDeclaration = _vertexDeclaration;
            _device.DrawPrimitives(PrimitiveTypeMapper.MapFrom(primitiveType), startIndex, primitiveCount);
        }

        public void Render(PrimitiveType primitiveType, int startIndex, int primitiveCount, TextureArea textureArea, float alpha, float intensity)
        {
            AssertBufferNotLocked();

            _device.SetStreamSource(0, _vertexBuffer, 0, _typeSize);
            _device.VertexDeclaration = _vertexDeclaration;

            var alphaColour = new Color4(alpha, 0, 0, 0);
            var intensityColour = new Color4(intensity, intensity, intensity);

            _device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
            _device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);

            _device.SetTextureStageState(0, TextureStage.Constant, intensityColour.ToArgb());
            _device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Constant);

            _device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Modulate);
            _device.SetTextureStageState(1, TextureStage.AlphaArg1, TextureArgument.Texture);

            _device.SetTextureStageState(1, TextureStage.Constant, alphaColour.ToArgb());
            _device.SetTextureStageState(1, TextureStage.AlphaArg2, TextureArgument.Constant);

            _device.SetTextureStageState(2, TextureStage.ColorOperation, TextureOperation.Disable);
            _device.SetTextureStageState(2, TextureStage.AlphaOperation, TextureOperation.Disable);

            _device.SetTexture(0, ((CachedTexture)textureArea).Texture);
            _device.DrawPrimitives(PrimitiveTypeMapper.MapFrom(primitiveType), startIndex, primitiveCount);
        }
    }
}