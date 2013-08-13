using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TexturedVertex4
    {
        [VertexDeclarationValue(VertexDeclarationValueType.Position)]
        public Vector4 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.TextureCoordinate)]
        public Vector2 TextureCoordinate;
    }
}
