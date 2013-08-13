using System.Runtime.InteropServices;
using SlimMath;

namespace Psy.Graphics.VertexDeclarations
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ColouredVertex4
    {
        [VertexDeclarationValue(VertexDeclarationValueType.Position)]
        public Vector3 Position;

        [VertexDeclarationValue(VertexDeclarationValueType.Colour)]
        public Color4 Colour;
    }
}
