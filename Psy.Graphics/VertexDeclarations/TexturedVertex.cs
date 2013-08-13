using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TexturedVertex
    {
        [VertexDeclarationValue(VertexDeclarationValueType.Position)]
        public Vector3 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.TextureCoordinate)]
        public Vector2 TextureCoordinate;
    }
}
