using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TransformedColouredTexturedVertex
    {
        [VertexDeclarationValue(VertexDeclarationValueType.PositionTransformed)]
        public Vector4 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.TextureCoordinate)]
        public Vector2 TextureCoordinate;

        [VertexDeclarationValue(VertexDeclarationValueType.Colour)]
        public Color4 Colour;

        public TransformedColouredTexturedVertex(Vector4 position, Vector2 textureCoordinate, Color4 colour) : this()
        {
            Colour = colour;
            Position = position;
            TextureCoordinate = textureCoordinate;
        }
    }
}
