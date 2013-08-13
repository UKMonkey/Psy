using Psy.Core;
using Psy.Graphics.VertexDeclarations;
using SlimMath;

namespace Psy.Graphics.Models
{
    public class MeshInstance
    {
        public TextureAreaHolder TextureArea;
        public readonly string TextureName;
        public readonly TexturedColouredVertex4[] VertexBuffer;
        public readonly int TriangleCount;

        public MeshInstance(string textureName, CompiledModel.Mesh mesh)
        {
            TextureName = textureName;
            VertexBuffer = new TexturedColouredVertex4[mesh.Triangles.Length * 3];
            TriangleCount = mesh.Triangles.Length;

            foreach (var triangle in mesh.Triangles.IndexOver())
            {
                VertexBuffer[triangle.Value.Vertex1Index].Colour = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
                VertexBuffer[triangle.Value.Vertex2Index].Colour = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
                VertexBuffer[triangle.Value.Vertex3Index].Colour = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
            }

            foreach (var vertex in mesh.Vertices.IndexOver())
            {
                VertexBuffer[vertex.Index].TextureCoordinate = mesh.TextureCoordinateBuffer[vertex.Index];
            }
        }
    }
}