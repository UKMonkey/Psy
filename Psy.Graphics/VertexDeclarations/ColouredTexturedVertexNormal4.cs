using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ColouredTexturedVertexNormal4
    {

        [VertexDeclarationValue(VertexDeclarationValueType.Position)] 
        public Vector4 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.Colour)] 
        public Color4 Colour;

        [VertexDeclarationValue(VertexDeclarationValueType.Normal)] 
        public Vector3 Normal;

        [VertexDeclarationValue(VertexDeclarationValueType.TextureCoordinate)] 
        public Vector2 Texture;

        public ColouredTexturedVertexNormal4(Vector4 position, Color4 colour, Vector3 normal, Vector2 texture)
            : this()
        {
            Position = position;
            Colour = colour;
            Normal = normal;
            Texture = texture;
        }
    }
}
