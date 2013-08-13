using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ColouredVertexNormal4
    {
        [VertexDeclarationValue(VertexDeclarationValueType.Position)]
        public Vector4 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.Colour)]
        public Color4 Colour;

        [VertexDeclarationValue(VertexDeclarationValueType.Normal)]
        public Vector3 Normal;
    }
}
