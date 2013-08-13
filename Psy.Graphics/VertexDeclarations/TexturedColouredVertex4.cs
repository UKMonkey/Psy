using System.Diagnostics;
using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential), DebuggerDisplay("Position={Position} TexCoord={TextureCoordinate}")]
    public struct TexturedColouredVertex4
    {
        [VertexDeclarationValue(VertexDeclarationValueType.Position)]
        public Vector4 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.TextureCoordinate)]
        public Vector2 TextureCoordinate;

        [VertexDeclarationValue(VertexDeclarationValueType.Colour)]
        public Color4 Colour;

        public TexturedColouredVertex4(Vector3 position, Color4 colour)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoordinate = new Vector2(0.0f, 0.0f);
            Colour = colour;
        }

        public TexturedColouredVertex4(Vector3 position, Vector2 textureCoordinate, Color4 colour)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoordinate = textureCoordinate;
            Colour = colour;
        }
    }
}
