using System;
using Psy.Core;

namespace Psy.Graphics
{
    public interface IVertexRenderer<T> : IDisposable where T : struct
    {
        IDataStream<T> LockVertexBuffer();
        void UnlockVertexBuffer();
        IDataStream<short> LockIndexBuffer();
        void UnlockIndexBuffer();

        // todo: remove base texture.
        void Render(PrimitiveType primitiveType, int startIndex, int primitiveCount);
        void RenderIndexed(PrimitiveType primitiveType, int primitiveCount);
        void RenderNonIndexed(PrimitiveType primitiveType, int startIndex, int primitiveCount);
        void RenderNoTexture(PrimitiveType primitiveType, int startIndex, int primitiveCount);
        void RenderForShader(PrimitiveType primitiveType, int startIndex, int primitiveCount);
        void Render(PrimitiveType primitiveType, int startIndex, 
                    int primitiveCount, TextureArea textureArea, float alpha, float intensity);
    }
}