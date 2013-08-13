using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TransformedColouredVertex
    {
        [VertexDeclarationValue(VertexDeclarationValueType.PositionTransformed)]
        public Vector4 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.Colour)]
        public Color4 Colour;

        public TransformedColouredVertex(Vector4 position, Color4 colour) : this()
        {
            Position = position;
            Colour = colour;
        }
    }
}
